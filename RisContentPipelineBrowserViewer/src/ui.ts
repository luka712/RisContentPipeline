import {FolderApi, Pane} from "tweakpane";
import * as EssentialsPlugin from '@tweakpane/plugin-essentials';
import {
    KHR_SUPERCOMPRESSION_BASISLZ, KHR_SUPERCOMPRESSION_NONE,
    KHR_SUPERCOMPRESSION_ZLIB,
    KHR_SUPERCOMPRESSION_ZSTD,
    type KTX2Container, VK_FORMAT_R8G8B8_SRGB, VK_FORMAT_R8G8B8_UNORM, VK_FORMAT_R8G8B8A8_SRGB, VK_FORMAT_R8G8B8A8_UNORM
} from "ktx-parse";


export class UI {

    private readonly _pane: Pane;
    private _ktx2Folder: FolderApi | undefined;
    private readonly _fileAddedCallback: ((file: File) => void)[] = [];

    private  readonly  _superompressionSchemeMap = {
        [KHR_SUPERCOMPRESSION_NONE] : "NONE",
        [KHR_SUPERCOMPRESSION_BASISLZ] : "BASISLZ",
        [KHR_SUPERCOMPRESSION_ZLIB] : "ZLIB",
        [KHR_SUPERCOMPRESSION_ZSTD] : "STD",
    }

    private readonly _vkFormatMap: { [key: number]: string } = {
        [VK_FORMAT_R8G8B8A8_UNORM]: "R8G8B8A8_UNORM",
        [VK_FORMAT_R8G8B8_UNORM]: "R8G8B8A8_UNORM",
        [VK_FORMAT_R8G8B8A8_SRGB]: "R8G8B8A8_SRGB",
        [VK_FORMAT_R8G8B8_SRGB]: "R8G8B8_SRGB",
    }

    constructor() {
        this._pane = new Pane();
        this._pane.registerPlugin(EssentialsPlugin);
        this._pane.addButton({
            title: "Add KTX2 File"
        }).on("click", () => {
            this._addFile();
        });
    }

    public onFileAdded(callback: (file: File) => void) {
        this._fileAddedCallback.push(callback);
    }

    public updateKtx2(container: KTX2Container) {

        if (!this._ktx2Folder) {
            this._ktx2Folder = this._pane.addFolder({
                title: 'KTX2 Info',
                expanded: true,   // optional
            });
        }


        const params = {
            "Dimension": `${container.pixelWidth}x${container.pixelHeight}`,
            "Supercompression Scheme": this._superompressionSchemeMap[container.supercompressionScheme],
            "Mip Levels" : container.levelCount.toString(),
            "VK Format" : this._vkFormatMap[container.vkFormat] ?? container.vkFormat?.toString() ?? "Unknown"
        }


        this._ktx2Folder.addBinding(params,"Dimension", {
            readonly: true
        });

        this._ktx2Folder.addBinding(params, "Supercompression Scheme", {
            readonly: true
        });
        this._ktx2Folder.addBinding(params, "Mip Levels", {
            readonly: true
        });
        this._ktx2Folder.addBinding(params, "VK Format", {
            readonly: true
        });
    }

    private _addFile() {

        const input = document.createElement("input");

        input.type = "file";
        input.accept = ".ktx2";

        input.onchange = async () => {
            const file = input.files?.[0];

            if (!file) {
                return;
            }

            for (const callback of this._fileAddedCallback) {
                callback(file);
            }
        };

        input.click();
    }

}