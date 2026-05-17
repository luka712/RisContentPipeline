import * as THREE from "three";
import type {FolderApi, Pane} from "tweakpane";
import {type AnyTexture, type TextureMeta, TextureType} from "../types.ts";
import type {MagnificationTextureFilter, MinificationTextureFilter} from "three";

interface TextureProperties {
    type: TextureType;
    dimensions: string,
    minFilter: MinificationTextureFilter,
    magFilter: MagnificationTextureFilter,
    anisotropy: number,
    sliceIndex: number,
}

const MIN_FILTER_OPTIONS = {
    Nearest: THREE.NearestFilter,
    Linear: THREE.LinearFilter,
    NearestMipmapNearest: THREE.NearestMipmapNearestFilter,
    NearestMipmapLinear: THREE.NearestMipmapLinearFilter,
    LinearMipmapNearest: THREE.LinearMipmapNearestFilter,
    LinearMipmapLinear: THREE.LinearMipmapLinearFilter,
} as const;

const MAG_FILTER_OPTIONS = {
    Nearest: THREE.NearestFilter,
    Linear: THREE.LinearFilter,
} as const;

export class TexturePropertiesPanel {

    private readonly _pane: Pane;
    private _folder: FolderApi;
    private _texture: AnyTexture | null = null;

    /** Params object for two-way Tweakpane bindings on the current texture. */
    private _textureProperties?: TextureProperties;

    constructor(pane: Pane,) {
        this._pane = pane;

        this._folder = pane.addFolder({
            title: 'Texture Properties',
            expanded: false
        });


    }

    /**
     * Wire the **Texture Properties** folder to the given texture and
     * metadata.
     *
     * All changes made in the UI are applied live to the texture object.
     *
     * @param texture - The currently displayed texture.
     * @param meta    - Normalised metadata (drives conditional controls).
     */
    public updateTexture(texture: AnyTexture, meta: TextureMeta): void {
        this._texture = texture;

        // Sync params from actual texture state
        this._textureProperties = {
            type: meta.type,
            dimensions: meta.width + " x " + meta.height,
            minFilter: texture.minFilter,
            magFilter: texture.magFilter,
            anisotropy: texture.anisotropy,
            sliceIndex: 0
        }

        this._createOrUpdateTextureFolder();
    }


    /** Rebuild the texture-properties folder (dispose old one first). */
    private _createOrUpdateTextureFolder(): void {
        if (this._folder) {
            this._folder.dispose();
        }

        this._folder = this._pane.addFolder({
            title: 'Texture Properties',
            expanded: true,
        });

        if(!this._textureProperties) {
            throw new Error("Texture properties object is not initialized.");
        }

        this._folder.addBinding(this._textureProperties, 'type', {
            readonly: true
        });

        this._folder.addBinding(this._textureProperties, 'dimensions', {
            readonly: true
        })

        this._folder
            .addBinding(this._textureProperties, 'minFilter', {
                options: MIN_FILTER_OPTIONS,
            })
            .on('change', (ev) => {
                if (this._texture) {
                    this._texture.minFilter =
                        ev.value as THREE.MinificationTextureFilter;
                    this._texture.needsUpdate = true;
                }
            });

        this._folder
            .addBinding(this._textureProperties, 'magFilter', {
                options: MAG_FILTER_OPTIONS,
            })
            .on('change', (ev) => {
                if (this._texture) {
                    this._texture.magFilter =
                        ev.value as THREE.MagnificationTextureFilter;
                    this._texture.needsUpdate = true;
                }
            });

        this._folder
            .addBinding(this._textureProperties, 'anisotropy', {
                min: 1,
                max: 16,
                step: 1,
            })
            .on('change', (ev) => {
                if (this._texture) {
                    this._texture.anisotropy = ev.value as number;
                    this._texture.needsUpdate = true;
                }
            });

    }

}