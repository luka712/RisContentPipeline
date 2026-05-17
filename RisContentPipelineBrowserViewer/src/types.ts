/**
 * Shared types and type guards for the Texture Inspector application.
 *
 * @module types
 */

import * as THREE from 'three';

/**
 * Classification of a texture based on its dimensionality and layer/face count.
 *
 * This drives the choice of Three.js geometry and material when displaying
 * the texture in the viewer.
 */
export const TextureType = {
    /** Single 2D texture (most common case). */
    Texture2D: '2D',

    /** 2D texture array – multiple slices that can be browsed. */
    Texture2DArray: '2D_ARRAY',

    /** Cubemap – 6 faces arranged as an environment map or skybox. */
    Cubemap: 'CUBEMAP',

    /** 3D volume texture. */
    Texture3D: '3D',
} as const;

/** Union of all texture type literals. */
export type TextureType = (typeof TextureType)[keyof typeof TextureType];

/**
 * Metadata describing a loaded texture, independent of the concrete
 * Three.js texture class.
 */
export interface TextureMeta {
    /** The resolved texture type. */
    type: TextureType;

    /** Width in pixels. */
    width: number;

    /** Height in pixels. */
    height: number;

    /** Number of mip-map levels (0 means only the base level). */
    mipLevels: number;

    /** Number of array layers (always 1 for 2D and cubemap textures). */
    layerCount: number;

    /** Number of faces (6 for cubemaps, otherwise 1). */
    faceCount: number;

    /** Pixel depth (greater than 1 for 3D textures). */
    depth: number;
}

/**
 * Union of all Three.js texture classes we may encounter after loading.
 */
export type AnyTexture =
    | THREE.Texture
    | THREE.CompressedTexture
    | THREE.CubeTexture
    | THREE.DataArrayTexture
    | THREE.Data3DTexture
    | THREE.CompressedArrayTexture;

/**
 * Union of all Three.js material classes used by the viewer.
 */
export type AnyMaterial =
    | THREE.MeshStandardMaterial
    | THREE.MeshBasicMaterial
    | THREE.ShaderMaterial;

/**
 * Determine the {@link TextureType} from raw KTX2 metadata.
 *
 * The heuristic follows the KTX2 specification:
 * - `faceCount === 6` → {@link TextureType.Cubemap}
 * - `layerCount > 1`  → {@link TextureType.Texture2DArray}
 * - `pixelDepth > 1`  → {@link TextureType.Texture3D}
 * - otherwise         → {@link TextureType.Texture2D}
 *
 * @param layerCount - Number of array layers (from KTX2 header).
 * @param faceCount  - Number of cubemap faces (from KTX2 header).
 * @param pixelDepth - Depth in pixels (from KTX2 header).
 * @returns The inferred texture type.
 */
export function classifyTextureType(
    layerCount: number,
    faceCount: number,
    pixelDepth: number,
): TextureType {
    if (faceCount === 6) {
        return TextureType.Cubemap;
    }
    if (layerCount > 1) {
        return TextureType.Texture2DArray;
    }
    if (pixelDepth > 1) {
        return TextureType.Texture3D;
    }
    return TextureType.Texture2D;
}

/**
 * Narrowing type guard for {@link THREE.CubeTexture}.
 *
 * @param texture - Any Three.js texture instance.
 * @returns `true` if the texture is a CubeTexture.
 */
export function isCubeTexture(
    texture: AnyTexture,
): texture is THREE.CubeTexture {
    return texture instanceof THREE.CubeTexture;
}

/**
 * Narrowing type guard for compressed texture variants.
 *
 * @param texture - Any Three.js texture instance.
 * @returns `true` if the texture is a compressed texture.
 */
export function isCompressedTexture(
    texture: AnyTexture,
): texture is THREE.CompressedTexture | THREE.CompressedArrayTexture {
    return (
        texture instanceof THREE.CompressedTexture ||
        texture instanceof THREE.CompressedArrayTexture
    );
}
