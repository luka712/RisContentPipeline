using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.Models;

/// <summary>
/// KTX2 export settings for image assets.
/// </summary>
public class Ktx2Settings
{
    /// <summary>
    /// The encoding target for KTX2 compression.
    /// </summary>
    public Ktx2EncodingTarget EncodeTarget { get; set; } = Ktx2EncodingTarget.BASIS_ETC1S;

    /// <summary>
    /// Whether to generate mipmaps.
    /// </summary>
    public bool GenerateMipmaps { get; set; } = true;

    /// <summary>
    /// Whether to flip the Y axis.
    /// </summary>
    public bool FlipY { get; set; }

    /// <summary>
    /// Quality level (0-5) for ETC1S encoding.
    /// </summary>
    public int QualityLevel { get; set; } = 3;

    /// <summary>
    /// Quality level (0-4) for UASTC encoding.
    /// </summary>
    public int UastcQualityLevel { get; set; } = 2;

    /// <summary>
    /// Gets the quality level value for ETC1S encoding.
    /// </summary>
    public int GetQualityLevelValue()
    {
        return QualityLevel switch
        {
            0 => 1,
            1 => 64,
            2 => 128,
            3 => 192,
            4 => 224,
            5 => 255,
            _ => 128
        };
    }

    /// <summary>
    /// Gets the UASTC quality level flags.
    /// </summary>
    public uint GetUastcQualityLevelValue()
    {
        return (uint)UastcQualityLevel;
    }

    /// <summary>
    /// Creates a copy of these settings.
    /// </summary>
    public Ktx2Settings Copy()
    {
        return new Ktx2Settings
        {
            EncodeTarget = EncodeTarget,
            GenerateMipmaps = GenerateMipmaps,
            FlipY = FlipY,
            QualityLevel = QualityLevel,
            UastcQualityLevel = UastcQualityLevel
        };
    }
}
