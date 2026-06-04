/**
 * Global type declarations for window methods exposed for external integration.
 *
 * These methods are available when the application is embedded in a WebView
 * or accessed from external JavaScript contexts.
 */

declare global {
    interface Window {
        /**
         * Load a KTX2 texture from a base64-encoded string.
         *
         * @param base64 - Raw base64 or data-URL prefixed string.
         * @param name - Optional name for the texture (defaults to "Unknown").
         *
         * @example
         * ```js
         * window.loadKtx2TextureFromBase64('AAAAFAAA...', 'MyTexture');
         * ```
         */
        loadKtx2TextureFromBase64(base64: string, name?: string): void;

        /**
         * Load a PNG texture from a base64-encoded string.
         *
         * @param base64 - Raw base64 or data-URL prefixed string.
         * @param name - Optional name for the texture (defaults to "Unknown").
         *
         * @example
         * ```js
         * window.loadPngTextureFromBase64('iVBORw0KGgo...', 'MyTexture');
         * ```
         */
        loadPngTextureFromBase64(base64: string, name?: string): void;

        /**
         * Load a KTX2 texture from raw bytes.
         *
         * @param bytes - Uint8Array containing the KTX2 file data.
         * @param name - Optional name for the texture (defaults to "Unknown").
         *
         * @example
         * ```js
         * const bytes = new Uint8Array([...]); // Your KTX2 file bytes
         * window.loadKtx2TextureFromBytes(bytes, 'MyTexture');
         * ```
         */
        loadKtx2TextureFromBytes(bytes: Uint8Array, name?: string): void;

        /**
         * Load a PNG texture from raw bytes.
         *
         * @param bytes - Uint8Array containing the PNG file data.
         * @param name - Optional name for the texture (defaults to "Unknown").
         *
         * @example
         * ```js
         * const bytes = new Uint8Array([...]); // Your PNG file bytes
         * window.loadPngTextureFromBytes(bytes, 'MyTexture');
         * ```
         */
        loadPngTextureFromBytes(bytes: Uint8Array, name?: string): void;

        /**
         * Clear all loaded textures from the viewer.
         *
         * @example
         * ```js
         * window.clearAllTextures();
         * ```
         */
        clearAllTextures(): void;
    }
}

export {};
