using RisKtx2;

namespace RisContentPipeline.Ktx2;

/// <summary>
/// The options for the KTX2 pipeline.
/// </summary>
public class Ktx2PipelineOptions
{
    /// <summary>
    /// The output path for the KTX2 texture.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;
    
    /// <summary>
    /// The encoding format for the KTX2 texture.
    /// </summary>
    public Ktx2EncodingTarget Encoding { get; set; } = Ktx2EncodingTarget.BASIS_ETC1S;
    
    /// <summary>
    /// The quality level for ASTC compression.
    /// </summary>
    public KtxPackAstcQualityLevels AstcQuality { get; set; } = KtxPackAstcQualityLevels.MEDIUM;
    
    /// <summary>
    /// The quality level for the KTX2 texture.
    /// Between 0 and 255, where 1 is the lowest quality and 255 is the highest quality.
    /// If <c>0</c>, the default quality level of <c>128</c> will be used.
    /// By default, the quality level is set to <c>0</c>.
    /// </summary>
    public uint QualityLevel { get; set; } = 0;

    /// <summary>
    /// TODO: fix documentation
    /// A set of <see cref="KtxUastcFlags"/> controlling UASTC encoding.
    /// The most important value is the level given in the least-significant 4 bits which selects a speed vs. quality tradeoff
    /// as shown in the following table:
    /// Level/Speed | Quality: 
    /// -----: |: -------:
    /// KTX_PACK_UASTC_LEVEL_FASTEST | 43.45dB
    /// KTX_PACK_UASTC_LEVEL_FASTER | 46.49dB
    /// <see cref="KtxUastcFlags.LEVEL_DEFAULT"/> | 47.47dB
    /// KTX_PACK_UASTC_LEVEL_SLOWER  | 48.01dB
    /// KTX_PACK_UASTC_LEVEL_VERYSLOW | 48.24dB
    /// </summary>
    public KtxUastcFlags UastcFlags { get; set; } = KtxUastcFlags.LEVEL_DEFAULT;
    
    /// <summary>
    /// Specifies whether to generate mipmaps for the KTX2 texture.
    /// </summary>
    public bool GenerateMipmaps { get; set; } = false;

    /// <summary>
    /// If <c>true</c> the image will be flipped vertically before encoding.
    /// This is useful for some graphics APIs that have different coordinate systems.
    /// </summary>
    public bool FlipY { get; set; } = false;
}