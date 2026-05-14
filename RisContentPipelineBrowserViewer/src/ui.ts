/**
 * UI layer built on top of Tweakpane.
 *
 * Exposes:
 * - A file picker button (accepts .ktx2 and .png)
 * - A read-only "KTX2 Info" section showing container metadata
 *   (only populated when a KTX2 file is loaded)
 * - A two-way "Texture Properties" section for editing material
 *   parameters (wrap, filter, anisotropy, flipY)
 * - Read-only renderer capability indicators
 */
import { FolderApi, Pane } from 'tweakpane';
import * as EssentialsPlugin from '@tweakpane/plugin-essentials';
import * as THREE from 'three';
import {
    KHR_DF_FLAG_ALPHA_PREMULTIPLIED,
    KHR_DF_MODEL_ASTC,
    KHR_DF_MODEL_ETC1S,
    KHR_DF_MODEL_ETC2,
    KHR_DF_MODEL_RGBSDA,
    KHR_DF_MODEL_UNSPECIFIED,
    KHR_DF_MODEL_UASTC,
    type KTX2Container,
} from 'ktx-parse';
import {Mapping} from "./mapping.ts";
import {UIConfig} from "./ui-config.ts";

// ---------------------------------------------------------------------------
// Mapping helpers for Three.js texture constants
// ---------------------------------------------------------------------------

const WRAP_OPTIONS = {
    'ClampToEdge': THREE.ClampToEdgeWrapping,
    'Repeat': THREE.RepeatWrapping,
    'MirroredRepeat': THREE.MirroredRepeatWrapping,
} as const;

const MIN_FILTER_OPTIONS = {
    'Nearest': THREE.NearestFilter,
    'Linear': THREE.LinearFilter,
    'NearestMipmapNearest': THREE.NearestMipmapNearestFilter,
    'NearestMipmapLinear': THREE.NearestMipmapLinearFilter,
    'LinearMipmapNearest': THREE.LinearMipmapNearestFilter,
    'LinearMipmapLinear': THREE.LinearMipmapLinearFilter,
} as const;

const MAG_FILTER_OPTIONS = {
    'Nearest': THREE.NearestFilter,
    'Linear': THREE.LinearFilter,
} as const;

/** Map from numeric KTX2 colour-model constant to a display label. */
const COLOR_MODEL_MAP: Record<number, string> = {
    [KHR_DF_MODEL_UNSPECIFIED]: 'Unspecified',
    [KHR_DF_MODEL_RGBSDA]: 'RGBSDA (traditional)',
    [KHR_DF_MODEL_ETC1S]: 'ETC1S',
    [KHR_DF_MODEL_ETC2]: 'ETC2',
    [KHR_DF_MODEL_ASTC]: 'ASTC',
    [KHR_DF_MODEL_UASTC]: 'UASTC',
};

// ---------------------------------------------------------------------------

export class UI {
    private readonly _pane: Pane;
    private _ktx2Folder: FolderApi | undefined;
    private _textureFolder: FolderApi | undefined;
    private readonly _fileAddedCallbacks: ((file: File) => void)[] = [];
    private _texture: THREE.Texture | null = null;
    private _material: THREE.Material | null = null;

    /** Params object for two-way Tweakpane bindings on the current texture. */
    private readonly _textureParams: Record<string, number | boolean> = {
        wrapS: THREE.ClampToEdgeWrapping,
        wrapT: THREE.ClampToEdgeWrapping,
        minFilter: THREE.LinearMipmapLinearFilter,
        magFilter: THREE.LinearFilter,
        anisotropy: 1,
        flipY: 1, // Tweakpane handles booleans as numbers internally
    };

    /** Cached renderer capabilities (filled by setRendererCaps). */
    private readonly _rendererCaps = {
        astc: false,
        etc1: false,
        etc2: false,
        s3tc: false,
        pvrtc: false,
    };

    constructor(config?: UIConfig) {

        config = config ?? new UIConfig();

        this._pane = new Pane({
           container: document.getElementById("pane") as HTMLElement,
        });

        this._pane.registerPlugin(EssentialsPlugin);

        if(config.useAddButton) {

            this._pane.addButton({
                title: 'Add File',
            }).on('click', () => {
                this._addFile();
            });
        }
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    /** Register a callback invoked when the user picks a file. */
    public onFileAdded(callback: (file: File) => void): void {
        this._fileAddedCallbacks.push(callback);
    }

    /**
     * Detect compression-format support from the renderer and cache it so
     * the information can be displayed in the UI.
     */
    public setRendererCaps(renderer: THREE.WebGLRenderer): void {
        const ext = renderer.extensions;
        this._rendererCaps.astc = ext.has('WEBGL_compressed_texture_astc');
        this._rendererCaps.etc1 = ext.has('WEBGL_compressed_texture_etc1');
        this._rendererCaps.etc2 = ext.has('WEBGL_compressed_texture_etc');
        this._rendererCaps.s3tc = ext.has('WEBGL_compressed_texture_s3tc');
        this._rendererCaps.pvrtc = ext.has('WEBGL_compressed_texture_pvrtc');
    }

    /**
     * Populate the "KTX2 Info" folder with metadata from a parsed KTX2
     * container.  Previous folder contents are disposed to avoid stale
     * bindings.
     */
    public updateKtx2(container: KTX2Container): void {
        // Dispose old folder so bindings don't accumulate
        if (this._ktx2Folder) {
            this._ktx2Folder.dispose();
        }

        this._ktx2Folder = this._pane.addFolder({
            title: 'KTX2 Info',
            expanded: true,
        });

        // --- Header / basic properties ---
        this._ktx2Folder.addBinding(
            { Dimension: `${container.pixelWidth} × ${container.pixelHeight}` },
            'Dimension',
            { readonly: true, label: 'Dimension' },
        );

        this._ktx2Folder.addBinding(
            { 'Layer Count': container.layerCount },
            'Layer Count',
            { readonly: true },
        );
        this._ktx2Folder.addBinding(
            { 'Face Count': container.faceCount },
            'Face Count',
            { readonly: true },
        );
        this._ktx2Folder.addBinding(
            { 'Pixel Depth': container.pixelDepth },
            'Pixel Depth',
            { readonly: true },
        );

        // --- Format info ---
        this._ktx2Folder.addBinding(
            { 'VK Format': Mapping.vkFormatMap[container.vkFormat] ?? container.vkFormat?.toString() ?? 'Unknown' },
            'VK Format',
            { readonly: true },
        );

        this._ktx2Folder.addBinding(
            { 'Type Size': container.typeSize },
            'Type Size',
            { readonly: true },
        );

        this._ktx2Folder.addBinding(
            { 'Supercompression': Mapping.supercompressionSchemeMap[container.supercompressionScheme] },
            'Supercompression',
            { readonly: true },
        );

        // --- Mip / level info ---
        this._ktx2Folder.addBinding(
            { 'Mip Levels': container.levelCount },
            'Mip Levels',
            { readonly: true },
        );

        const totalLevelDataSize = container.levels.reduce(
            (sum, level) => sum + level.levelData.byteLength, 0,
        );
        this._ktx2Folder.addBinding(
            { 'Total Data': `${(totalLevelDataSize / 1024).toFixed(1)} KB` },
            'Total Data',
            { readonly: true },
        );

        // --- DFD (Data Format Descriptor) ---
        const dfd = container.dataFormatDescriptor?.[0];
        if (dfd) {
            const dfdParams: Record<string, string> = {
                'Color Model': COLOR_MODEL_MAP[dfd.colorModel] ?? `Unknown (${dfd.colorModel})`,
                'Transfer': Mapping.transferMap[dfd.transferFunction] ?? `Unknown (${dfd.transferFunction})`,
                'Primaries': Mapping.primariesMap[dfd.colorPrimaries] ?? `Unknown (${dfd.colorPrimaries})`,
                'Alpha Mode': Mapping.alphaMap[dfd.flags & KHR_DF_FLAG_ALPHA_PREMULTIPLIED] ?? 'Unknown',
            };

            this._ktx2Folder.addBinding(dfdParams, 'Color Model', { readonly: true });
            this._ktx2Folder.addBinding(dfdParams, 'Transfer', { readonly: true });
            this._ktx2Folder.addBinding(dfdParams, 'Primaries', { readonly: true });
            this._ktx2Folder.addBinding(dfdParams, 'Alpha Mode', { readonly: true });
        }

        // --- Key/Value metadata ---
        const kvCount = Object.keys(container.keyValue).length;
        this._ktx2Folder.addBinding(
            { 'Key/Value Pairs': kvCount },
            'Key/Value Pairs',
            { readonly: true },
        );

        // --- Supercompression Global Data (BasisLZ) ---
        if (container.globalData) {
            this._ktx2Folder.addBinding(
                { 'Endpoints': container.globalData.endpointCount },
                'Endpoints',
                { readonly: true },
            );
            this._ktx2Folder.addBinding(
                { 'Selectors': container.globalData.selectorCount },
                'Selectors',
                { readonly: true },
            );
        }
    }

    /**
     * Remove the KTX2 info folder from the UI.
     *
     * Call this when loading a non-KTX2 texture (e.g. PNG) so the stale
     * metadata section does not remain visible.
     */
    public clearKtx2(): void {
        if (this._ktx2Folder) {
            this._ktx2Folder.dispose();
            this._ktx2Folder = undefined;
        }
    }

    /**
     * Wire the "Texture Properties" folder to the given texture and material.
     * All changes made in the UI are applied live to both objects.
     */
    public updateTexture(texture: THREE.Texture, material: THREE.Material): void {
        this._texture = texture;
        this._material = material;

        // Sync params from actual texture state (cast to number for
        // Tweakpane compatibility)
        this._textureParams.wrapS = texture.wrapS as number;
        this._textureParams.wrapT = texture.wrapT as number;
        this._textureParams.minFilter = texture.minFilter as number;
        this._textureParams.magFilter = texture.magFilter as number;
        this._textureParams.anisotropy = texture.anisotropy;
        this._textureParams.flipY = texture.flipY ? 1 : 0;

        this._createOrUpdateTextureFolder();
    }

    // -----------------------------------------------------------------------
    // Texture property folder (two-way bindings)
    // -----------------------------------------------------------------------

    /** Rebuild the texture-properties folder (dispose old one first). */
    private _createOrUpdateTextureFolder(): void {
        if (this._textureFolder) {
            this._textureFolder.dispose();
        }

        this._textureFolder = this._pane.addFolder({
            title: 'Texture Properties',
            expanded: true,
        });

        this._textureFolder.addBinding(this._textureParams, 'wrapS', {
            options: WRAP_OPTIONS,
        }).on('change', (ev) => {
            if (this._texture) {
                this._texture.wrapS = ev.value as THREE.Wrapping;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        this._textureFolder.addBinding(this._textureParams, 'wrapT', {
            options: WRAP_OPTIONS,
        }).on('change', (ev) => {
            if (this._texture) {
                this._texture.wrapT = ev.value as THREE.Wrapping;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        this._textureFolder.addBinding(this._textureParams, 'minFilter', {
            options: MIN_FILTER_OPTIONS,
        }).on('change', (ev) => {
            if (this._texture) {
                this._texture.minFilter = ev.value as THREE.MinificationTextureFilter;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        this._textureFolder.addBinding(this._textureParams, 'magFilter', {
            options: MAG_FILTER_OPTIONS,
        }).on('change', (ev) => {
            if (this._texture) {
                this._texture.magFilter = ev.value as THREE.MagnificationTextureFilter;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        this._textureFolder.addBinding(this._textureParams, 'anisotropy', {
            min: 1,
            max: 16,
            step: 1,
        }).on('change', (ev) => {
            if (this._texture) {
                this._texture.anisotropy = ev.value as number;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        this._textureFolder.addBinding(this._textureParams, 'flipY').on('change', (ev) => {
            if (this._texture) {
                this._texture.flipY = !!ev.value;
                this._texture.needsUpdate = true;
                if (this._material) this._material.needsUpdate = true;
            }
        });

        // Renderer capabilities info (read-only)
        const capsParams = {
            ASTC: this._rendererCaps.astc,
            ETC1: this._rendererCaps.etc1,
            ETC2: this._rendererCaps.etc2,
            S3TC: this._rendererCaps.s3tc,
            PVRTC: this._rendererCaps.pvrtc,
        };

        this._textureFolder.addBinding(capsParams, 'ASTC', { readonly: true });
        this._textureFolder.addBinding(capsParams, 'ETC1', { readonly: true });
        this._textureFolder.addBinding(capsParams, 'ETC2', { readonly: true });
        this._textureFolder.addBinding(capsParams, 'S3TC', { readonly: true });
        this._textureFolder.addBinding(capsParams, 'PVRTC', { readonly: true });
    }

    // -----------------------------------------------------------------------
    // File input
    // -----------------------------------------------------------------------

    /** Open a native file picker dialog and fire callbacks on selection. */
    private _addFile(): void {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.ktx2,.png';

        input.onchange = () => {
            const file = input.files?.[0];
            if (!file) return;

            for (const callback of this._fileAddedCallbacks) {
                callback(file);
            }
        };

        input.click();
    }
}