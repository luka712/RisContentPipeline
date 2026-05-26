import {TextureViewer} from "./viewer.ts";
import type {LoadedTexture} from "./texture-loader.ts";
import type {UiManager} from "./ui/ui-manager.ts";
import type {KTX2Container} from "ktx-parse";
import type {AnyTexture, TextureMeta} from "./types.ts";

export class LoadedTextureExtended implements LoadedTexture {

    constructor(loadedTexture: LoadedTexture) {
        this.meta = loadedTexture.meta;
        this.ktxContainer = loadedTexture.ktxContainer;
        this.texture = loadedTexture.texture;
    }

    /** @inheritDoc */
    public readonly ktxContainer: KTX2Container | null;

    /** @inheritDoc */
    public readonly meta: TextureMeta;

    /** @inheritDoc */
    public readonly texture: AnyTexture;

    /**
     * Custom action to be performed by TextureListPanel.
     */
    public callback: () => void = () => {
    };

}

export class TextureList {

    private readonly _ui: UiManager;
    private readonly _viewer: TextureViewer;
    private _textures: LoadedTextureExtended[] = [];

    // TODO: doc comment
    public constructor(ui: UiManager, viewer: TextureViewer) {
        this._ui = ui;
        this._viewer = viewer;
    }

    private _showKtx2Texture(loadedTexture: LoadedTextureExtended) {
        this._viewer.setTexture(loadedTexture.texture, loadedTexture.meta);
        this._ui.texturePropertiesPanel.updateTexture(loadedTexture.texture, loadedTexture.meta);
        if (loadedTexture.ktxContainer) {
            this._ui.updateKtx2(loadedTexture.ktxContainer);
        }
        this._ui.textureListPanel.addTextures(this._textures);
    }

    // TODO: doc comment
    public addKtx2Texture(loadedTexture: LoadedTexture): void {

        const loadedTxtExt = new LoadedTextureExtended(loadedTexture);
        loadedTxtExt.callback = () => {
            this._showKtx2Texture(loadedTxtExt);
        }
        this._textures.push(loadedTxtExt);
        this._showKtx2Texture(loadedTxtExt);
    }


    private _showTexture(loadedTexture: LoadedTextureExtended): void {
        this._viewer.setTexture(loadedTexture.texture, loadedTexture.meta);
        this._ui.texturePropertiesPanel.updateTexture(loadedTexture.texture, loadedTexture.meta);
        this._ui.clearKtx2();
        this._ui.textureListPanel.addTextures(this._textures);
    }

    public addTexture(loadedTexture: LoadedTexture): void {
        const loadedTxtExt = new LoadedTextureExtended(loadedTexture)
        loadedTxtExt.callback = () => {
            this._showTexture(loadedTxtExt);
        }
        this._textures.push(loadedTxtExt);
        this._showTexture(loadedTxtExt);
    }

    // TODO: doc comment
    public clear() {
        this._ui.textureListPanel.clear();
        this._textures = [];
    }

}