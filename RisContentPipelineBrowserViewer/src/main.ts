import * as THREE from 'three';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';
import {KTX2Loader} from "three/addons/loaders/KTX2Loader.js";
import {UI} from "./ui.ts";
import {read} from "ktx-parse";

// --------------------------------------------
// Scene
// --------------------------------------------

const ui = new UI();

ui.onFileAdded(async (file) => {

    loadKTX2(file);

    // Show ktx2 info
    const data = await file.bytes();
    const ktxContainer = read(data);

    ui.updateKtx2(ktxContainer);
})

const scene = new THREE.Scene();
scene.background = new THREE.Color(0x222222);

const camera = new THREE.PerspectiveCamera(
    60,
    window.innerWidth / window.innerHeight,
    0.1,
    100
);

camera.position.set(0, 0, 2);

const renderer = new THREE.WebGLRenderer({
    antialias: true
});

renderer.setSize(window.innerWidth, window.innerHeight);
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

// const geometry = new THREE.BoxGeometry(1, 1, 1);
const geometry = new THREE.PlaneGeometry(1, 1);

const material = new THREE.MeshStandardMaterial({
    color: 0xffffff, side: THREE.DoubleSide,
});

const mesh = new THREE.Mesh(geometry, material);
// mesh.rotation.x = Math.PI / 2;
scene.add(mesh);

// --------------------------------------------
// KTX2 Loader
// --------------------------------------------

const ktx2Loader = new KTX2Loader();

ktx2Loader.setTranscoderPath(
    'https://unpkg.com/three@0.181.1/examples/jsm/libs/basis/'
);

ktx2Loader.detectSupport(renderer);

// --------------------------------------------
// UI
// --------------------------------------------

const info = document.getElementById('info');

// --------------------------------------------
// File Loading
// --------------------------------------------

const button = document.getElementById("open-file-btn") as HTMLButtonElement;

button.addEventListener("click", async () => {

});


function loadKTX2(file: File) {

    const objectURL = URL.createObjectURL(file);

    ktx2Loader.load(
        objectURL,

        (texture) => {

            texture.flipY = true;
            material.map = texture;
            material.needsUpdate = true;
            material.transparent = true;

            writeTextureInfo(texture, file);

            URL.revokeObjectURL(objectURL);
        },

        undefined,

        (err) => {
            console.error(err);
        }
    );
}

// --------------------------------------------
// Texture Info
// --------------------------------------------

function writeTextureInfo(texture, file) {

    const rendererCaps = renderer.capabilities;

    const output = {
        fileName: file.name,
        fileSizeBytes: file.size,

        width: texture.image.width,
        height: texture.image.height,

        format: texture.format,
        type: texture.type,
        colorSpace: texture.colorSpace,

        mipmaps: texture.mipmaps
            ? texture.mipmaps.length
            : 0,

        minFilter: texture.minFilter,
        magFilter: texture.magFilter,

        anisotropy: texture.anisotropy,

        generateMipmaps: texture.generateMipmaps,

        compressed: texture.isCompressedTexture === true,

        renderer: {
            astc: rendererCaps.astc,
            etc1: rendererCaps.etc1,
            etc2: rendererCaps.etc2,
            s3tc: rendererCaps.s3tc,
            pvrtc: rendererCaps.pvrtc
        }
    };

    info.textContent =
        JSON.stringify(output, null, 4);
}

// --------------------------------------------
// Resize
// --------------------------------------------

window.addEventListener('resize', () => {

    camera.aspect =
        window.innerWidth / window.innerHeight;

    camera.updateProjectionMatrix();

    renderer.setSize(
        window.innerWidth,
        window.innerHeight
    );
});

// --------------------------------------------
// Render Loop
// --------------------------------------------

function animate() {

    requestAnimationFrame(animate);

    renderer.render(scene, camera);
}

animate();