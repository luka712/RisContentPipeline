using RisKtx2;

namespace RisContentPipeline.Ktx2;

/// <summary>
/// The options for the KTX2 pipeline.
/// </summary>
public class Ktx2PipelineOptions
{
    private uint _qualityLevel = 128;
    
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
    /// Between 1 and 255, where 1 is the lowest quality and 255 is the highest quality.
    /// By default, the quality level is set to <c>128</c>.
    /// </summary>
    public uint QualityLevel
    {
        get => _qualityLevel;
        set => _qualityLevel = Math.Clamp(value, 1, 255);
    }

    /// <summary>
    /// A set of <see cref="KtxUastcFlags"/> controlling UASTC encoding.
    /// The most important value is the level given in the least-significant 4 bits which selects a speed vs. quality tradeoff
    /// as shown in the following table:
    /// <list type="table">
    ///   <listheader>
    ///     <term>Level/Speed</term>
    ///     <description>Quality (PSNR)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>KTX_PACK_UASTC_LEVEL_FASTEST</term>
    ///     <description>43.45 dB</description>
    ///   </item>
    ///   <item>
    ///     <term>KTX_PACK_UASTC_LEVEL_FASTER</term>
    ///     <description>46.49 dB</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="KtxUastcFlags.LEVEL_DEFAULT"/></term>
    ///     <description>47.47 dB</description>
    ///   </item>
    ///   <item>
    ///     <term>KTX_PACK_UASTC_LEVEL_SLOWER</term>
    ///     <description>48.01 dB</description>
    ///   </item>
    ///   <item>
    ///     <term>KTX_PACK_UASTC_LEVEL_VERYSLOW</term>
    ///     <description>48.24 dB</description>
    ///   </item>
    /// </list>
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