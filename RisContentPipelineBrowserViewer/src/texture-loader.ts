/**
 * High-level texture loading API.
 *
 * Encapsulates all Three.js loader instantiation, KTX2 transcoder path
 * configuration, and the bookkeeping required to map from a generic Blob
 * to a concrete Three.js texture instance.
 *
 * The loader supports four texture categories:
 * - **2D** textures (single image)
 * - **2D array** textures (multiple layers)
 * - **Cubemap** textures (6 faces)
 * - **3D** volume textures
 *
 * @module texture-loader
 */

import * as THREE from 'three';
import { KTX2Loader } from 'three/addons/loaders/KTX2Loader.js';
import { read, type KTX2Container } from 'ktx-parse';
import {
    classifyTextureType,
    type AnyTexture,
    type TextureMeta,
    TextureType,
} from './types.js';

/** Payload delivered when a texture is successfully loaded. */
export interface LoadedTexture {
    /** The concrete Three.js texture instance. */
    texture: AnyTexture;

    /** Normalised metadata extracted from the texture / container. */
    meta: TextureMeta;

    /** Raw KTX2 container when the source was a `.ktx2` file; otherwise `null`. */
    ktxContainer: KTX2Container | null;
}

/** Signature for per-texture-type load callbacks. */
export type OnLoadCallback = (result: LoadedTexture) => void;

/** Signature for load-error callbacks. */
export type OnErrorCallback = (err: unknown) => void;

/**
 * Configuration passed to the {@link TextureLoader} constructor.
 */
export interface TextureLoaderConfig {
    /** The WebGL renderer used to detect supported compressed formats. */
    renderer: THREE.WebGLRenderer;

    /**
     * Base URL pointing to the Basis Universal transcoder binaries.
     *
     * @default 'https://unpkg.com/three@0.184.0/examples/jsm/libs/basis/'
     */
    transcoderPath?: string;
}

/**
 * Orchestrates loading of KTX2 and PNG textures through Three.js loaders.
 *
 * Usage:
 * ```ts
 * const loader = new TextureLoader({ renderer });
 * loader.loadKtx2(file, (result) => {
 *     sceneMaterial.map = result.texture;
 * });
 * ```
 */
export class TextureLoader {
    private readonly _ktx2Loader: KTX2Loader;

    constructor(config: TextureLoaderConfig) {
        this._ktx2Loader = new KTX2Loader();
        this._ktx2Loader.setTranscoderPath(
            config.transcoderPath ??
                'https://unpkg.com/three@0.184.0/examples/jsm/libs/basis/',
        );
        this._ktx2Loader.detectSupport(config.renderer);
    }

    // ------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------

    /**
     * Load a KTX2 file (`.ktx2`) and resolve to a {@link LoadedTexture}.
     *
     * The method first parses the raw bytes with `ktx-parse` to determine
     * the texture type, then delegates to Three.js {@link KTX2Loader} for
     * the actual GPU upload.
     *
     * @param file   - The KTX2 blob (from a File input or base64 decode).
     * @param onLoad - Called on successful decode & upload.
     * @param onError - Called when decoding or upload fails.
     */
    loadKtx2(
        file: Blob,
        onLoad: OnLoadCallback,
        onError?: OnErrorCallback,
    ): void {
        const objectURL = URL.createObjectURL(file);

        this._ktx2Loader.load(
            objectURL,
            (texture: THREE.CompressedTexture) => {
                // KTX2Loader may return CubeTexture, DataArrayTexture,
                // Data3DTexture, etc. at runtime — cast to the union.
                this._resolveKtx2(
                    file,
                    texture as AnyTexture,
                    onLoad,
                    onError,
                );
                URL.revokeObjectURL(objectURL);
            },
            undefined,
            (err: unknown) => {
                URL.revokeObjectURL(objectURL);
                onError?.(err);
            },
        );
    }

    /**
     * Load a conventional image file (PNG, JPEG, WebP, etc.) via
     * {@link THREE.TextureLoader}.
     *
     * Only 2D textures are supported for non-KTX2 sources.
     *
     * @param file    - The image blob.
     * @param onLoad  - Called on successful decode & upload.
     * @param onError - Called when decoding or upload fails.
     */
    loadImage(
        file: Blob,
        onLoad: OnLoadCallback,
        onError?: OnErrorCallback,
    ): void {
        const objectURL = URL.createObjectURL(file);
        const loader = new THREE.TextureLoader();

        loader.load(
            objectURL,
            (texture: THREE.Texture) => {
                const img = texture.image as { width: number; height: number };
                onLoad({
                    texture,
                    meta: {
                        type: TextureType.Texture2D,
                        width: img.width,
                        height: img.height,
                        mipLevels: texture.mipmaps?.length ?? 1,
                        layerCount: 1,
                        faceCount: 1,
                        depth: 1,
                    },
                    ktxContainer: null,
                });
                URL.revokeObjectURL(objectURL);
            },
            undefined,
            (err: unknown) => {
                URL.revokeObjectURL(objectURL);
                onError?.(err);
            },
        );
    }

    // ------------------------------------------------------------------
    // Internal helpers
    // ------------------------------------------------------------------

    /**
     * Parse the raw KTX2 bytes, derive metadata, and forward the result.
     */
    private async _resolveKtx2(
        file: Blob,
        texture: AnyTexture,
        onLoad: OnLoadCallback,
        _onError?: OnErrorCallback,
    ): Promise<void> {
        const data = await file.bytes();
        const container = read(data);

        const type = classifyTextureType(
            container.layerCount,
            container.faceCount,
            container.pixelDepth,
        );

        // Flip Y for 2D textures; cubemaps and arrays use different conventions
        if (type === TextureType.Texture2D) {
            (texture as THREE.Texture).flipY = true;
        }

        onLoad({
            texture,
            meta: {
                type,
                width: container.pixelWidth,
                height: container.pixelHeight,
                mipLevels: container.levelCount,
                layerCount: container.layerCount,
                faceCount: container.faceCount,
                depth: container.pixelDepth,
            },
            ktxContainer: container,
        });
    }
}
