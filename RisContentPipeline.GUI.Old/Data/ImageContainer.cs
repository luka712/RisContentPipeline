using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Data;

/// <summary>
/// The extended image container that includes additional settings for KTX2 export.
/// </summary>
internal class ImageContainer
{
    /// <summary>
    /// The settings for exporting the image to KTX2.
    /// </summary>
    public Ktx2Settings Ktx2ExportSettings { get; set; } = new();
    
    /// <summary>
    /// The file path of the image.
    /// </summary>
    public string? FilePath { get; set; }
        
    /// <summary>
    /// The file name of the image.
    /// </summary>
    public string? FileName => Path.GetFileName(FilePath);

    /// <summary>
    /// The file type of the image file.
    /// </summary>
    public string? FileType
    {
        get
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                return null;
            }
            return Path.GetExtension(FilePath);
        }
    }
    
}