import {ButtonApi, type FolderApi, type Pane} from "tweakpane";
import type {LoadedTextureExtended} from "../texture-list.ts";




export  class TextureListPanel {
    private  readonly _folder:FolderApi;

    private readonly _buttons: ButtonApi[] = [];

    /**
     * The constructor.
     * @param pane The pane.
     */
    constructor(pane: Pane) {

        this._folder = pane.addFolder({
            title: 'Texture List',
            expanded: false,
        });
        this._folder.element.title = 'Texture List';
    }


    // TODO: doc comment
    public addTextures(textures: LoadedTextureExtended[]) {

        this.clear();

        // Add button for each texture.
        for(const texture of textures) {

            const button = this._folder.addButton({
                title: texture.texture.userData.name
            });
            button.on('click', texture.callback);
           this._buttons.push(button);
        }
    }

    /**
     * Remove all texture buttons from this control.
     */
    public clear(): void {

        for(const button of this._buttons) {
            this._folder.remove(button);
        }
    }

}