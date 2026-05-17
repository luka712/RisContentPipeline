import * as THREE from "three";
import type {FolderApi, Pane} from "tweakpane";

/**
 * Displays the information about user GPU.
 */
export class GpuPanel {

    private  readonly  _renderer: THREE.WebGLRenderer;
    private  readonly _folder:FolderApi;
    private readonly _supportedCompressionExtensions: FolderApi;

    /**
     * The constructor.
     * @param pane The pane.
     * @param pane The pane.
     * @param renderer The renderer.
     */
    constructor(pane: Pane, renderer: THREE.WebGLRenderer) {

        this._renderer = renderer;
        this._folder = pane.addFolder({
            title: 'GPU Properties',
            expanded: false,
        });
        this._folder.element.title = 'GPU Properties';

        this._setGpuInfo();

        this._supportedCompressionExtensions = this._folder.addFolder({
            title: 'Supported Compression Formats',
            expanded: true,
        })
        this._supportedCompressionExtensions.element.title = 'Suported Compression Formats';

        this._renderer = renderer;
        this._setTextureCompressionParams();
    }

    private _setGpuInfo() {
        const gl = this._renderer.getContext();
        const debugInfo = gl.getExtension("WEBGL_debug_renderer_info");

        if (debugInfo) {
            const debug = {
                vendor: gl.getParameter(debugInfo.UNMASKED_VENDOR_WEBGL),
                gpu: gl.getParameter(debugInfo.UNMASKED_RENDERER_WEBGL)
            }

            let binding = this._folder.addBinding(debug, 'vendor', {
                readonly: true
            });
            binding.element.title = debug.vendor;

            binding = this._folder.addBinding(debug, "gpu", {
                readonly: true
            });
            binding.element.title = debug.gpu;
        }
    }

    private _setTextureCompressionParams(){

        const ext = this._renderer.extensions;

        const params = [];

        // 1. Modern mobile standard (Android + Apple)
        params.push({
            format: 'ASTC',
            tooltip:
                'ASTC (Adaptive Scalable Texture Compression). \n' +
                'Modern high-efficiency texture format for mobile GPUs. \n' +
                'Preferred when supported due to best quality-to-memory ratio.',
            ASTC: ext.has('WEBGL_compressed_texture_astc'),
        });

        // 2. Universal mobile baseline
        params.push({
            format: 'ETC2',
            tooltip:
                'ETC2 texture compression. Widely supported baseline format on mobile GPUs. \n' +
                'Used as a fallback when ASTC is not available. \n',
            ETC2: ext.has('WEBGL_compressed_texture_etc'),
        });

        // 3. High-end desktop quality format
        params.push({
            format: 'BPTC',
            tooltip:
                'BPTC (Block Compression). High-quality format for modern desktop GPUs.\n' +
                'Suitable for high-fidelity textures where supported.\n',
            BPTC: ext.has('EXT_texture_compression_bptc'),
        });

        // 4. Legacy but widely supported desktop format
        params.push({
            format: 'S3TC',
            tooltip:
                'S3TC (DXT compression family). Widely supported legacy desktop format.\n' +
                'Common fallback for broad compatibility across older hardware.\n',
            S3TC: ext.has('WEBGL_compressed_texture_s3tc'),
        });

        // 5. Color-space variant for S3TC
        params.push({
            format: 'S3TC_SRGB',
            tooltip:
                'S3TC format with sRGB color encoding.\n' +
                'Used for color textures requiring correct gamma handling.\n',
            S3TC_SRGB: ext.has('WEBGL_compressed_texture_s3tc_srgb'),
        });

        // 6. Legacy Apple-specific format
        params.push({
            format: 'PVRTC',
            tooltip:
                'PVRTC texture compression.\n Legacy format primarily used on older Apple GPUs.\n' +
                'Included for backward compatibility.',
            PVRTC: ext.has('WEBGL_compressed_texture_pvrtc'),
        });

        for(let param of params) {
            let binding = this._supportedCompressionExtensions.addBinding(param, param.format as any, {
                readonly: true,
            });
            binding.element.title = param.tooltip;
        }
    }
}