using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.Dto;

/// <summary>
/// The KTX2 settings DTO.
/// </summary>
public record Ktx2SettingsDto
{
    /// <summary>
    /// The encoding target.
    /// </summary>
    public Ktx2EncodingTarget EncodingTarget { get; set; }
    
    /// <summary>
    /// The quality level.
    /// </summary>
    public int QualityLevel { get; set; }
    
    /// <summary>
    /// The generate mipmaps flag.
    /// </summary>
    public bool GenerateMipmaps { get; set; }
    
    /// <summary>
    /// The flip Y flag.
    /// </summary>
    public bool FlipY { get; set; }
}