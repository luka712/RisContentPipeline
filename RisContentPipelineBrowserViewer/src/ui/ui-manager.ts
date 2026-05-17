/**
 * UI layer built on top of Tweakpane.
 *
 * Exposes:
 * - A file picker button (accepts `.ktx2` and `.png`)
 * - A read-only **KTX2 Info** section showing container metadata
 *   (only populated when a KTX2 file is loaded)
 * - A **Texture Properties** section for editing sampler state
 *   (wrap, filter, anisotropy, flipY)
 * - A **Slice** slider for array / 3D textures
 * - Read-only renderer capability indicators
 *
 * @module ui
 */

import { FolderApi, Pane } from 'tweakpane';
import * as EssentialsPlugin from '@tweakpane/plugin-essentials';
import * as THREE from 'three';
import { type KTX2Container } from 'ktx-parse';
import { extractKtx2Info } from '../texture-info.js';
import { UIConfig } from './ui-config.js';
import {GpuPanel} from "./gpu-panel.ts";
import {TexturePropertiesPanel} from "./texture-properties-panel.ts";

// ---------------------------------------------------------------------------


export class UiManager {
    private readonly _pane: Pane;
    private _ktx2Folder: FolderApi | undefined;
    private readonly _fileAddedCallbacks: ((file: File) => void)[] = [];
    private readonly _sliceChangeCallbacks: ((value: number) => void)[] = [];


    constructor(renderer: THREE.WebGLRenderer, config?: UIConfig) {
        config = config ?? new UIConfig();

        this._pane = new Pane({
            container: document.getElementById('pane') as HTMLElement,
        });

        this._pane.registerPlugin(EssentialsPlugin);

        if (config.useAddButton) {
            this._pane
                .addButton({
                    title: 'Add File',
                })
                .on('click', () => {
                    this._addFile();
                });
        }

        new GpuPanel(this._pane, renderer);
        this.texturePropertiesPanel = new TexturePropertiesPanel(this._pane);

    }

    public readonly texturePropertiesPanel: TexturePropertiesPanel;

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    /** Register a callback invoked when the user picks a file. */
    public onFileAdded(callback: (file: File) => void): void {
        this._fileAddedCallbacks.push(callback);
    }

    /**
     * Register a callback invoked when the slice / depth slider changes.
     *
     * Relevant for array and 3D textures only.
     */
    public onSliceChanged(callback: (value: number) => void): void {
        this._sliceChangeCallbacks.push(callback);
    }


    /**
     * Populate the **KTX2 Info** folder with metadata from a parsed KTX2
     * container.  Previous folder contents are disposed to avoid stale
     * bindings.
     */
    public updateKtx2(container: KTX2Container): void {
        this._disposeKtx2Folder();

        const info = extractKtx2Info(container);


        this._ktx2Folder = this._pane.addFolder({
            title: 'KTX2 Info',
            expanded: true,
        });

        this._addReadonlyBinding(this._ktx2Folder, 'Dimension', info.dimension);
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Layer Count',
            info.layerCount,
        );
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Face Count',
            info.faceCount,
        );
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Pixel Depth',
            info.pixelDepth,
        );
        this._addReadonlyBinding(this._ktx2Folder, 'VK Format', info.vkFormat);
        this._addReadonlyBinding(this._ktx2Folder, 'Type Size', info.typeSize);
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Supercompression',
            info.supercompression,
        );
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Mip Levels',
            info.mipLevels,
        );
        this._addReadonlyBinding(this._ktx2Folder, 'Total Data', info.totalDataKB);
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Color Model',
            info.colorModel,
        );
        this._addReadonlyBinding(this._ktx2Folder, 'Transfer', info.transfer);
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Primaries',
            info.primaries,
        );
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Alpha Mode',
            info.alphaMode,
        );
        this._addReadonlyBinding(
            this._ktx2Folder,
            'Key/Value Pairs',
            info.keyValuePairs,
        );

        if (info.endpoints !== undefined) {
            this._addReadonlyBinding(
                this._ktx2Folder,
                'Endpoints',
                info.endpoints,
            );
        }
        if (info.selectors !== undefined) {
            this._addReadonlyBinding(
                this._ktx2Folder,
                'Selectors',
                info.selectors,
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
        this._disposeKtx2Folder();
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

    // -----------------------------------------------------------------------
    // Internal helpers
    // -----------------------------------------------------------------------

    private _disposeKtx2Folder(): void {
        if (this._ktx2Folder) {
            this._ktx2Folder.dispose();
            this._ktx2Folder = undefined;
        }
    }

    private _addReadonlyBinding(
        folder: FolderApi,
        label: string,
        value: string | number,
    ): void {
        const proxy = { [label]: value };
        folder.addBinding(proxy, label, { readonly: true });
    }
}
