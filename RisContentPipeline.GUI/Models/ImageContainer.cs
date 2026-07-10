namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Container for image asset data and its KTX2 export settings.
/// </summary>
public class ImageContainer
{
    /// <summary>
    /// The file path of the image.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// The KTX2 export settings for this image.
    /// </summary>
    public Ktx2Settings Ktx2ExportSettings { get; set; } = new();
}
