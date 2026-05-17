/**
 * Helpers for extracting human-readable metadata from KTX2 containers.
 *
 * This module centralises all string-formatting logic so that the UI layer
 * stays declarative and free of low-level KTX2 arithmetic.
 *
 * @module texture-info
 */

import { type KTX2Container } from 'ktx-parse';
import { Mapping } from './mapping.js';

/** Aggregated info object produced from a parsed KTX2 container. */
export interface Ktx2Info {
    dimension: string;
    layerCount: number;
    faceCount: number;
    pixelDepth: number;
    vkFormat: string;
    typeSize: number;
    supercompression: string;
    mipLevels: number;
    totalDataKB: string;
    colorModel: string;
    transfer: string;
    primaries: string;
    alphaMode: string;
    keyValuePairs: number;
    endpoints: number | undefined;
    selectors: number | undefined;
}

/**
 * Extract a flat {@link Ktx2Info} object from a parsed KTX2 container.
 *
 * All numeric constants are resolved to human-readable labels via
 * {@link Mapping} where possible.
 *
 * @param container - Parsed KTX2 container (from `ktx-parse`).
 * @returns Fully populated info object ready for UI binding.
 */
export function extractKtx2Info(container: KTX2Container): Ktx2Info {
    const totalLevelDataSize = container.levels.reduce(
        (sum, level) => sum + level.levelData.byteLength,
        0,
    );

    const dfd = container.dataFormatDescriptor?.[0];

    return {
        dimension: `${container.pixelWidth} × ${container.pixelHeight}`,
        layerCount: container.layerCount,
        faceCount: container.faceCount,
        pixelDepth: container.pixelDepth,
        vkFormat:
            Mapping.vkFormatMap[container.vkFormat] ??
            container.vkFormat?.toString() ??
            'Unknown',
        typeSize: container.typeSize,
        supercompression:
            Mapping.supercompressionSchemeMap[container.supercompressionScheme] ??
            'Unknown',
        mipLevels: container.levelCount,
        totalDataKB: `${(totalLevelDataSize / 1024).toFixed(1)} KB`,
        colorModel:
            (dfd && Mapping.colorModelMap?.[dfd.colorModel]) ??
            `Unknown (${dfd?.colorModel})`,
        transfer:
            (dfd && Mapping.transferMap[dfd.transferFunction]) ??
            `Unknown (${dfd?.transferFunction})`,
        primaries:
            (dfd && Mapping.primariesMap[dfd.colorPrimaries]) ??
            `Unknown (${dfd?.colorPrimaries})`,
        alphaMode:
            (dfd && Mapping.alphaMap[dfd.flags]) ?? 'Unknown',
        keyValuePairs: Object.keys(container.keyValue).length,
        endpoints: container.globalData?.endpointCount,
        selectors: container.globalData?.selectorCount,
    };
}
