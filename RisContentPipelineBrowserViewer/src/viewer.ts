/**
 * Three.js scene manager that adapts geometry and materials to the texture
 * type being inspected.
 *
 * Supported display modes:
 * - **2D** – textured plane
 * - **Cubemap** – skybox background + reflective sphere
 * - **Array** – textured plane with a slice-index shader
 * - **3D** – textured plane with a depth-slice shader
 *
 * @module viewer
 */

import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import {
    type AnyTexture,
    type TextureMeta,
    TextureType,
} from './types.js';

/** Vertex shader shared by the custom array / 3D slice materials. */
const SLICE_VERTEX_SHADER = /* glsl */ `
varying vec2 vUv;
void main() {
    vUv = uv;
    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
}
`;

/** Fragment shader for 2D-array texture slice preview. */
const ARRAY_FRAGMENT_SHADER = /* glsl */ `
uniform sampler2DArray uTexture;
uniform float uLayer;
varying vec2 vUv;

void main() {
    gl_FragColor = texture(uTexture, vec3(vUv, uLayer));
}
`;

/** Fragment shader for 3D texture depth-slice preview. */
const VOLUME_FRAGMENT_SHADER = /* glsl */ `
uniform sampler3D uTexture;
uniform float uDepth;
varying vec2 vUv;

void main() {
    gl_FragColor = texture(uTexture, vec3(vUv, uDepth));
}
`;

/**
 * Configuration passed to the {@link TextureViewer} constructor.
 */
export interface ViewerConfig {
    /** The canvas element that will host the WebGL context. */
    canvas: HTMLCanvasElement;

    /** Background colour for the scene (defaults to dark grey). */
    backgroundColor?: number;
}

/**
 * Manages the Three.js scene, camera, renderer, and the mesh used to display
 * the currently inspected texture.
 *
 * The viewer automatically switches display strategy based on the texture type
 * (2D, cubemap, array, or 3D).
 */
export class TextureViewer {
    private readonly _canvas: HTMLCanvasElement;
    private readonly _scene: THREE.Scene;
    private readonly _camera: THREE.PerspectiveCamera;
    private readonly _renderer: THREE.WebGLRenderer;
    private readonly _controls: OrbitControls;
    private _mesh: THREE.Mesh | null = null;
    private _currentTexture: AnyTexture | null = null;
    private _currentMeta: TextureMeta | null = null;

    constructor(config: ViewerConfig) {
        this._canvas = config.canvas;
        this._canvas.width = window.innerWidth;
        this._canvas.height = window.innerHeight;

        this._scene = new THREE.Scene();
        this._scene.background = new THREE.Color(
            config.backgroundColor ?? 0x222222,
        );

        this._camera = new THREE.PerspectiveCamera(
            60,
            this._canvas.width / this._canvas.height,
            0.1,
            100,
        );
        this._camera.position.set(0, 0, 1);

        this._renderer = new THREE.WebGLRenderer({
            antialias: true,
            canvas: this._canvas,
        });
        this._renderer.setSize(this._canvas.width, this._canvas.height);

        this._controls = new OrbitControls(
            this._camera,
            this._renderer.domElement,
        );

        this._setupLighting();
        this._setupResizeListener();
        this._startRenderLoop();
    }

    // ------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------

    /** Read-only access to the WebGL renderer (for capability detection). */
    get renderer(): THREE.WebGLRenderer {
        return this._renderer;
    }

    /** The currently displayed texture (if any). */
    get currentTexture(): AnyTexture | null {
        return this._currentTexture;
    }

    /** Metadata describing the currently displayed texture (if any). */
    get currentMeta(): TextureMeta | null {
        return this._currentMeta;
    }

    /**
     * Replace the displayed texture.
     *
     * The viewer will create the appropriate geometry and material for the
     * texture type and add it to the scene.
     *
     * @param texture - The Three.js texture to display.
     * @param meta    - Normalised metadata (width, height, type, …).
     */
    setTexture(texture: AnyTexture, meta: TextureMeta): void {
        this._disposeCurrentDisplay();

        this._currentTexture = texture;
        this._currentMeta = meta;

        switch (meta.type) {
            case TextureType.Cubemap:
                this._createCubemapDisplay(texture);
                break;
            case TextureType.Texture2DArray:
                this._createArrayDisplay(texture, meta);
                break;
            case TextureType.Texture3D:
                this._createVolumeDisplay(texture, meta);
                break;
            default:
                this._create2DDisplay(texture, meta);
                break;
        }
    }

    /**
     * Update the slice / depth uniform for array or 3D textures.
     *
     * Has no effect when a 2D texture or cubemap is active.
     *
     * @param sliceIndex – Integer layer index (array) or depth slice (3D).
     */
    setSliceIndex(sliceIndex: number): void {
        if (!this._mesh) return;
        const mat = this._mesh.material as THREE.ShaderMaterial;

        if (mat?.uniforms?.uLayer) {
            mat.uniforms.uLayer.value = sliceIndex;
        }

        if (mat?.uniforms?.uDepth && this._currentMeta) {
            // Convert integer slice to normalised depth coordinate
            const depth = this._currentMeta.depth;
            mat.uniforms.uDepth.value = (sliceIndex + 0.5) / depth;
        }
    }

    /** Remove the current texture and reset to an empty scene. */
    clear(): void {
        this._disposeCurrentDisplay();
        this._currentTexture = null;
        this._currentMeta = null;
    }

    // ------------------------------------------------------------------
    // Display builders
    // ------------------------------------------------------------------

    /** Build a simple plane for 2D textures. */
    private _create2DDisplay(
        texture: AnyTexture,
        meta: TextureMeta,
    ): void {
        const geometry = new THREE.PlaneGeometry(1, 1);
        const material = new THREE.MeshStandardMaterial({
            color: 0xffffff,
            side: THREE.DoubleSide,
            map: texture as THREE.Texture,
        });

        this._mesh = new THREE.Mesh(geometry, material);
        this._fitAspectRatio(meta.width, meta.height);
        this._scene.add(this._mesh);
    }

    /** Build a skybox background + reflective sphere for cubemaps. */
    private _createCubemapDisplay(texture: AnyTexture): void {
        const cubeTex = texture as THREE.CubeTexture;

        // Skybox background
        this._scene.background = cubeTex;

        // Reflective sphere so the user can orbit and inspect reflections
        const geometry = new THREE.SphereGeometry(0.4, 64, 64);
        const material = new THREE.MeshStandardMaterial({
            color: 0xffffff,
            metalness: 1.0,
            roughness: 0.0,
            envMap: cubeTex,
        });

        this._mesh = new THREE.Mesh(geometry, material);
        this._scene.add(this._mesh);
    }

    /** Build a custom-shaded plane for 2D-array textures. */
    private _createArrayDisplay(
        texture: AnyTexture,
        meta: TextureMeta,
    ): void {
        const geometry = new THREE.PlaneGeometry(1, 1);
        const material = new THREE.ShaderMaterial({
            uniforms: {
                uTexture: { value: texture },
                uLayer: { value: 0 },
            },
            vertexShader: SLICE_VERTEX_SHADER,
            fragmentShader: ARRAY_FRAGMENT_SHADER,
            side: THREE.DoubleSide,
        });

        this._mesh = new THREE.Mesh(geometry, material);
        this._fitAspectRatio(meta.width, meta.height);
        this._scene.add(this._mesh);
    }

    /** Build a custom-shaded plane for 3D volume textures. */
    private _createVolumeDisplay(
        texture: AnyTexture,
        meta: TextureMeta,
    ): void {
        const geometry = new THREE.PlaneGeometry(1, 1);
        const material = new THREE.ShaderMaterial({
            uniforms: {
                uTexture: { value: texture },
                uDepth: { value: 0.5 },
            },
            vertexShader: SLICE_VERTEX_SHADER,
            fragmentShader: VOLUME_FRAGMENT_SHADER,
            side: THREE.DoubleSide,
        });

        this._mesh = new THREE.Mesh(geometry, material);
        this._fitAspectRatio(meta.width, meta.height);
        this._scene.add(this._mesh);
    }

    // ------------------------------------------------------------------
    // Scene helpers
    // ------------------------------------------------------------------

    private _setupLighting(): void {
        this._scene.add(new THREE.AmbientLight(0xffffff, 1.0));

        const dirLight = new THREE.DirectionalLight(0xffffff, 2.0);
        dirLight.position.set(2, 2, 2);
        this._scene.add(dirLight);
    }

    /** Scale the plane / mesh so it matches the texture's aspect ratio. */
    private _fitAspectRatio(width: number, height: number): void {
        if (!this._mesh) return;
        if (width > height) {
            this._mesh.scale.x = width / height;
            this._mesh.scale.y = 1;
        } else {
            this._mesh.scale.x = 1;
            this._mesh.scale.y = height / width;
        }
    }

    /** Remove and dispose the current mesh and reset scene state. */
    private _disposeCurrentDisplay(): void {
        if (this._mesh) {
            this._scene.remove(this._mesh);
            this._mesh.geometry.dispose();

            const mat = this._mesh.material;
            const mats = Array.isArray(mat) ? mat : [mat];

            for (const m of mats) {
                // Null out texture references before disposal so we don't
                // accidentally destroy a texture that may be reused elsewhere.
                if (m instanceof THREE.MeshStandardMaterial) {
                    m.map = null;
                    m.envMap = null;
                }
                if (m instanceof THREE.MeshBasicMaterial) {
                    m.map = null;
                    m.envMap = null;
                }
                m.dispose();
            }

            this._mesh = null;
        }

        // Reset background (cubemaps set this)
        this._scene.background = new THREE.Color(0x222222);
    }

    // ------------------------------------------------------------------
    // Lifecycle
    // ------------------------------------------------------------------

    private _setupResizeListener(): void {
        window.addEventListener('resize', () => {
            this._camera.aspect = window.innerWidth / window.innerHeight;
            this._camera.updateProjectionMatrix();
            this._renderer.setSize(window.innerWidth, window.innerHeight);
        });
    }

    private _startRenderLoop(): void {
        const loop = (): void => {
            this._controls.update();
            this._renderer.render(this._scene, this._camera);
            requestAnimationFrame(loop);
        };
        loop();
    }
}
