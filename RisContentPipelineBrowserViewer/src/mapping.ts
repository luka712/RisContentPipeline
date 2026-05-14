/**
 * Human-readable label mappings for KTX2 container metadata values.
 *
 * These maps translate numeric constants from the KTX2 specification (and the
 * `ktx-parse` library) into descriptive strings for display in the UI.
 */
import {
    KHR_DF_FLAG_ALPHA_PREMULTIPLIED,
    KHR_DF_FLAG_ALPHA_STRAIGHT, KHR_DF_PRIMARIES_BT2020, KHR_DF_PRIMARIES_BT709,
    KHR_DF_PRIMARIES_DISPLAYP3, KHR_DF_PRIMARIES_UNSPECIFIED,
    KHR_DF_TRANSFER_HLG_EOTF, KHR_DF_TRANSFER_LINEAR, KHR_DF_TRANSFER_PQ_EOTF,
    KHR_DF_TRANSFER_SRGB, KHR_DF_TRANSFER_UNSPECIFIED,
    KHR_SUPERCOMPRESSION_BASISLZ, KHR_SUPERCOMPRESSION_NONE, KHR_SUPERCOMPRESSION_ZLIB,
    KHR_SUPERCOMPRESSION_ZSTD,
    VK_FORMAT_ASTC_4x4_SRGB_BLOCK,
    VK_FORMAT_ASTC_4x4_UNORM_BLOCK, VK_FORMAT_ASTC_6x6_SRGB_BLOCK, VK_FORMAT_ASTC_6x6_UNORM_BLOCK,
    VK_FORMAT_ASTC_8x8_SRGB_BLOCK,
    VK_FORMAT_ASTC_8x8_UNORM_BLOCK,
    VK_FORMAT_BC1_RGBA_SRGB_BLOCK,
    VK_FORMAT_BC1_RGBA_UNORM_BLOCK, VK_FORMAT_BC3_SRGB_BLOCK, VK_FORMAT_BC3_UNORM_BLOCK, VK_FORMAT_BC4_UNORM_BLOCK,
    VK_FORMAT_BC5_UNORM_BLOCK, VK_FORMAT_BC6H_UFLOAT_BLOCK, VK_FORMAT_BC7_SRGB_BLOCK, VK_FORMAT_BC7_UNORM_BLOCK,
    VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK, VK_FORMAT_ETC2_R8G8B8A8_SRGB_BLOCK, VK_FORMAT_ETC2_R8G8B8A8_UNORM_BLOCK,
    VK_FORMAT_PVRTC1_4BPP_SRGB_BLOCK_IMG,
    VK_FORMAT_PVRTC1_4BPP_UNORM_BLOCK_IMG,
    VK_FORMAT_R8G8B8_SRGB,
    VK_FORMAT_R8G8B8_UNORM,
    VK_FORMAT_R8G8B8A8_SRGB,
    VK_FORMAT_R8G8B8A8_UNORM,
    VK_FORMAT_UNDEFINED
} from "ktx-parse";

export class Mapping {

    /** Map of supercompression scheme constants to human-readable labels. */
    public static readonly supercompressionSchemeMap = {
        [KHR_SUPERCOMPRESSION_NONE]: 'NONE',
        [KHR_SUPERCOMPRESSION_BASISLZ]: 'BASISLZ',
        [KHR_SUPERCOMPRESSION_ZLIB]: 'ZLIB',
        [KHR_SUPERCOMPRESSION_ZSTD]: 'STD',
    };

    /** Map of Vulkan format constants to human-readable labels. */
    public static readonly vkFormatMap: Record<number, string> = {
        [VK_FORMAT_UNDEFINED]: 'UNDEFINED (Basis Universal)',
        [VK_FORMAT_R8G8B8A8_UNORM]: 'R8G8B8A8_UNORM',
        [VK_FORMAT_R8G8B8_UNORM]: 'R8G8B8_UNORM',
        [VK_FORMAT_R8G8B8A8_SRGB]: 'R8G8B8A8_SRGB',
        [VK_FORMAT_R8G8B8_SRGB]: 'R8G8B8_SRGB',
        [VK_FORMAT_BC1_RGBA_UNORM_BLOCK]: 'BC1_RGBA_UNORM',
        [VK_FORMAT_BC1_RGBA_SRGB_BLOCK]: 'BC1_RGBA_SRGB',
        [VK_FORMAT_BC3_UNORM_BLOCK]: 'BC3_UNORM',
        [VK_FORMAT_BC3_SRGB_BLOCK]: 'BC3_SRGB',
        [VK_FORMAT_BC4_UNORM_BLOCK]: 'BC4_UNORM',
        [VK_FORMAT_BC5_UNORM_BLOCK]: 'BC5_UNORM',
        [VK_FORMAT_BC6H_UFLOAT_BLOCK]: 'BC6H_UFLOAT',
        [VK_FORMAT_BC7_UNORM_BLOCK]: 'BC7_UNORM',
        [VK_FORMAT_BC7_SRGB_BLOCK]: 'BC7_SRGB',
        [VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK]: 'ETC2_R8G8B8_UNORM',
        [VK_FORMAT_ETC2_R8G8B8A8_UNORM_BLOCK]: 'ETC2_R8G8B8A8_UNORM',
        [VK_FORMAT_ETC2_R8G8B8A8_SRGB_BLOCK]: 'ETC2_R8G8B8A8_SRGB',
        [VK_FORMAT_ASTC_4x4_UNORM_BLOCK]: 'ASTC_4x4_UNORM',
        [VK_FORMAT_ASTC_4x4_SRGB_BLOCK]: 'ASTC_4x4_SRGB',
        [VK_FORMAT_ASTC_6x6_UNORM_BLOCK]: 'ASTC_6x6_UNORM',
        [VK_FORMAT_ASTC_6x6_SRGB_BLOCK]: 'ASTC_6x6_SRGB',
        [VK_FORMAT_ASTC_8x8_UNORM_BLOCK]: 'ASTC_8x8_UNORM',
        [VK_FORMAT_ASTC_8x8_SRGB_BLOCK]: 'ASTC_8x8_SRGB',
        [VK_FORMAT_PVRTC1_4BPP_UNORM_BLOCK_IMG]: 'PVRTC1_4BPP_UNORM',
        [VK_FORMAT_PVRTC1_4BPP_SRGB_BLOCK_IMG]: 'PVRTC1_4BPP_SRGB',
    };

    /** Map of alpha mode flags to human-readable labels. */
    public static readonly alphaMap: Record<number, string> = {
        [KHR_DF_FLAG_ALPHA_STRAIGHT]: 'Straight',
        [KHR_DF_FLAG_ALPHA_PREMULTIPLIED]: 'Premultiplied',
    };

    /** Map of colour primaries constants to human-readable labels. */
    public static readonly primariesMap: Record<number, string> = {
        [KHR_DF_PRIMARIES_UNSPECIFIED]: 'Unspecified',
        [KHR_DF_PRIMARIES_BT709]: 'BT.709 (sRGB)',
        [KHR_DF_PRIMARIES_BT2020]: 'BT.2020',
        [KHR_DF_PRIMARIES_DISPLAYP3]: 'Display P3',
    };

    /** Map of transfer function constants to human-readable labels. */
    public  static readonly transferMap: Record<number, string> = {
        [KHR_DF_TRANSFER_UNSPECIFIED]: 'Unspecified',
        [KHR_DF_TRANSFER_LINEAR]: 'Linear',
        [KHR_DF_TRANSFER_SRGB]: 'sRGB',
        [KHR_DF_TRANSFER_PQ_EOTF]: 'PQ (HDR10)',
        [KHR_DF_TRANSFER_HLG_EOTF]: 'HLG (HDR)',
    };
}