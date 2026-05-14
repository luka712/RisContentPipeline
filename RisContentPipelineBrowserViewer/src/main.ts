/**
 * Entry point for the KTX2 / PNG Texture Inspector.
 *
 * - Sets up a Three.js scene with orbit controls and lighting.
 * - Uses KTX2Loader for `.ktx2` files and TextureLoader for `.png` files.
 * - Exposes a `window.loadKtx2TextureFromBase64` entry point for external
 *   integration (e.g. from the RisContentPipeline).
 */
import * as THREE from 'three';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';
import {KTX2Loader} from 'three/addons/loaders/KTX2Loader.js';
import {UI} from './ui.ts';
import {read} from 'ktx-parse';

// --------------------------------------------
// Scene
// --------------------------------------------

const ui = new UI({
    useAddButton: false,
});

/** Shared handler invoked when a file is selected via the UI. */
ui.onFileAdded(async (file) => {
    // Detect file type by extension
    const isKTX2 = file.name.toLowerCase().endsWith('.ktx2');

    if (isKTX2) {
        loadKTX2(file);

        // Show KTX2 container info in the UI
        const data = await file.bytes();
        const ktxContainer = read(data);
        ui.updateKtx2(ktxContainer);
    } else {
        // PNG (or any other image the browser can natively decode)
        loadPNG(file);

        // Remove stale KTX2 info if present
        ui.clearKtx2();
    }
});

const canvas = document.getElementById('viewer-canvas') as HTMLCanvasElement;
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x222222);

const camera = new THREE.PerspectiveCamera(
    60,
    canvas.width / canvas.height,
    0.1,
    100,
);

camera.position.set(0, 0, 1);

const renderer = new THREE.WebGLRenderer({
    antialias: true,
    canvas: canvas,
});
renderer.setSize(canvas.width, canvas.height);
document.body.appendChild(renderer.domElement);

const controls = new OrbitControls(camera, renderer.domElement);

// --------------------------------------------
// Lighting
// --------------------------------------------

scene.add(new THREE.AmbientLight(0xffffff, 1.0));

const dirLight = new THREE.DirectionalLight(0xffffff, 2.0);
dirLight.position.set(2, 2, 2);
scene.add(dirLight);

// --------------------------------------------
// Mesh
// --------------------------------------------

const geometry = new THREE.PlaneGeometry(1, 1);
const material = new THREE.MeshStandardMaterial({
    color: 0xffffff,
    side: THREE.DoubleSide,
});
const mesh = new THREE.Mesh(geometry, material);
scene.add(mesh);

// --------------------------------------------
// KTX2 Loader
// --------------------------------------------

const ktx2Loader = new KTX2Loader();
ktx2Loader.setTranscoderPath(
    'https://unpkg.com/three@0.184.0/examples/jsm/libs/basis/',
);
ktx2Loader.detectSupport(renderer);

// Pass renderer to detect compression format support
ui.setRendererCaps(renderer);

// --------------------------------------------
// File Loading
// --------------------------------------------

/**
 * Load a KTX2 file via the KTX2Loader and apply the resulting compressed
 * texture to the mesh material.
 */
function loadKTX2(file: Blob): void {
    const objectURL = URL.createObjectURL(file);

    ktx2Loader.load(
        objectURL,
        async (texture: THREE.CompressedTexture) => {
            texture.flipY = true;
            material.map = texture;
            material.needsUpdate = true;
            material.transparent = true;

            const img = texture.image as { width: number; height: number };
            fitAspectRatio(img.width, img.height);

            // Two-way texture property controls
            ui.updateTexture(texture, material);

            // Show KTX2 container info in the UI
            const data = await file.bytes();
            const ktxContainer = read(data);
            ui.updateKtx2(ktxContainer);

            URL.revokeObjectURL(objectURL);
        },

        undefined,

        (err: unknown) => {
            console.error(err);
        },
    );
}

/**
 * Load a regular image file (PNG, JPEG, etc.) via Three.js TextureLoader and
 * apply it to the mesh material.
 */
function loadPNG(file: Blob): void {
    const objectURL = URL.createObjectURL(file);

    const loader = new THREE.TextureLoader();
    loader.load(
        objectURL,

        (texture: THREE.Texture) => {
            texture.flipY = true;
            material.map = texture;
            material.needsUpdate = true;
            material.transparent = false; // reset; user can toggle via UI

            const img = texture.image as { width: number; height: number };
            fitAspectRatio(img.width, img.height);

            // Two-way texture property controls
            ui.updateTexture(texture, material);

            URL.revokeObjectURL(objectURL);
        },

        undefined,

        (err: unknown) => {
            console.error(err);
        },
    );
}

/**
 * Scale the plane geometry so it matches the texture's aspect ratio.
 */
function fitAspectRatio(width: number, height: number): void {
    if (width > height) {
        mesh.scale.x = width / height;
        mesh.scale.y = 1;
    } else {
        mesh.scale.x = 1;
        mesh.scale.y = height / width;
    }
}

// --------------------------------------------
// Resize
// --------------------------------------------

window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
});

// --------------------------------------------
// Render Loop
// --------------------------------------------

function animate(): void {

    controls.update();
    renderer.render(scene, camera);
    requestAnimationFrame(animate);
}

animate();

function base64ToBlob(base64: string, mimeType: string): Blob {

    // Remove data URL prefix if present
    const cleaned = base64.includes(',')
        ? base64.split(',')[1]
        : base64;

    const binary = atob(cleaned);

    const bytes = new Uint8Array(binary.length);

    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }

    return new Blob([bytes], {
        type: mimeType,
    });
}

/**
 * Global entry point for loading a KTX2 texture from a base64-encoded string.
 *
 * @example
 * ```js
 * window.loadKtx2TextureFromBase64('AAAAFAAA...');
 * ```
 */
// @ts-ignore
window.loadKtx2TextureFromBase64 = function (base64: string): void {

    const blob = base64ToBlob(base64, 'application/octet-stream');
    loadKTX2(blob);
};

// @ts-ignore
window.loadPngTextureFromBase64 = function (base64: string): void {
    const blob = base64ToBlob(base64, 'image/png');
    loadPNG(blob);
}