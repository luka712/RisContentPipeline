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
    /// The quality level for UASTC compression.
    /// Between <c>0</c> and <c>4</c>, where <c>0</c> is the lowest quality and <c>4</c> is the highest quality.
    /// By default, the quality level is set to <c>2</c>.
    /// </summary>
    public uint UastcQuality { get; set; } = 2;
    
    /// <summary>
    /// Specifies whether to generate mipmaps for the KTX2 texture.
    /// </summary>
    public bool GenerateMipmaps { get; set; } = false;
    
}