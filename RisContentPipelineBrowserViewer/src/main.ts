/**
 * Entry point for the KTX2 / PNG Texture Inspector.
 *
 * Architecture:
 * - {@link TextureViewer} manages the Three.js scene, camera, renderer, and
 *   adapts the display geometry to the texture type (2D, cubemap, array, 3D).
 * - {@link TextureLoader} loads `.ktx2` and `.png` files and produces a
 *   normalised {@link LoadedTexture} payload.
 * - {@link UiManager} (Tweakpane) provides file pickers, read-only metadata panels,
 *   live sampler-state editing, and slice/depth sliders for multi-layer
 *   textures.
 *
 * External integration (embedded WebView):
 * ```js
 * window.loadKtx2TextureFromBase64('AAAAFAAA...');
 * window.loadPngTextureFromBase64('iVBORw0KGgo...');
 * window.loadKtx2TextureFromBytes(new Uint8Array([...]));
 * window.loadPngTextureFromBytes(new Uint8Array([...]));
 * ```
 *
 * @module main
 */

import {UiManager} from './ui/ui-manager';
import {type LoadedTexture, TextureLoader} from './texture-loader.js';
import {TextureViewer} from './viewer.js';
import {TextureList} from "./texture-list.ts";


// ---------------------------------------------------------------------------
// Viewer (scene, camera, renderer)
// ---------------------------------------------------------------------------

const canvas = document.getElementById('viewer-canvas') as HTMLCanvasElement;
const viewer = new TextureViewer({canvas});

// ---------------------------------------------------------------------------
// Loader (KTX2 + PNG)
// ---------------------------------------------------------------------------

const textureLoader = new TextureLoader({
    renderer: viewer.renderer,
});

// UI
const ui = new UiManager(viewer.renderer, {
    useAddButton: true,
});

const textureList = new TextureList(ui, viewer);


// ---------------------------------------------------------------------------
// File loading pipeline
// ---------------------------------------------------------------------------

ui.onFileAdded(async (file) => {
    const isKtx2 = file.name.toLowerCase().endsWith('.ktx2');

    if (isKtx2) {
        textureLoader.loadKtx2(
            file,
            (result) => {
                result.texture.userData = {
                    name: file.name,
                }
                textureList.addKtx2Texture(result);
            },
            (err) => console.error('KTX2 load error:', err),
        );
    } else {
        textureLoader.loadImage(
            file,
            (result) => {
                result.texture.userData = {
                    name: file.name,
                }
                textureList.addTexture(result);
            },
            (err) => console.error('Image load error:', err),
        );
    }
});

// ---------------------------------------------------------------------------
// Slice / depth slider → viewer
// ---------------------------------------------------------------------------

ui.onSliceChanged((value) => {
    viewer.setSliceIndex(value);
});

// ---------------------------------------------------------------------------
// Base64 helpers (embedded WebView integration)
// ---------------------------------------------------------------------------

/**
 * Decode a base64 string into a Blob with the given MIME type.
 *
 * Handles both raw base64 and data-URL prefixed strings.
 */
function base64ToBlob(base64: string, mimeType: string): Blob {
    const cleaned = base64.includes(',') ? base64.split(',')[1] : base64;
    const binary = atob(cleaned);
    const bytes = new Uint8Array(binary.length);

    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }

    return new Blob([bytes], {type: mimeType});
}

/**
 * Global entry point for loading a KTX2 texture from a base64-encoded string.
 *
 * @param base64 - Raw base64 or data-URL prefixed string.
 * @param name - The optional name for texture.
 *
 * @example
 * ```js
 * window.loadKtx2TextureFromBase64('AAAAFAAA...');
 * ```
 */
window.loadKtx2TextureFromBase64 = function ( base64: string, name: string = "Unknown"): void {
    const blob = base64ToBlob(base64, 'application/octet-stream');
    textureLoader.loadKtx2(
        blob,
        (result) => {
            result.texture.userData = {
                name: name
            };
            textureList.addKtx2Texture(result);
        },
        (err) => console.error('Base64 KTX2 load error:', err),
    );
};

/**
 * Global entry point for loading a PNG texture from a base64-encoded string.
 *
 * @param base64 - Raw base64 or data-URL prefixed string.
 * @param name - The optional name for texture.
 *
 * @example
 * ```js
 * window.loadPngTextureFromBase64('iVBORw0KGgo...');
 * ```
 */
window.loadPngTextureFromBase64 = function (base64: string, name: string = "Unknown"): void {
    const blob = base64ToBlob(base64, 'image/png');
    textureLoader.loadImage(
        blob,
        (result) => {
            result.texture.userData = {
                name: name
            }
            textureList.addTexture(result);
        },
        (err) => console.error('Base64 PNG load error:', err),
    );
};

window.clearAllTextures = function(){
    textureList.clear();
}
