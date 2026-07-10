namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Represents a file or folder asset in the content pipeline.
/// </summary>
public class AssetFileOrFolder
{
    /// <summary>
    /// The absolute path to the file or folder.
    /// </summary>
    public string AbsolutePathOrFileName { get; set; } = string.Empty;

    /// <summary>
    /// The image container if this is an image file.
    /// </summary>
    public ImageContainer? Image { get; set; }

    /// <summary>
    /// Gets the file name or path without the directory.
    /// </summary>
    public string PathOrFileName => Path.GetFileName(AbsolutePathOrFileName);

    /// <summary>
    /// Gets whether this asset is a JSON file.
    /// </summary>
    public bool IsJson => AbsolutePathOrFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this asset is an XML file.
    /// </summary>
    public bool IsXml => AbsolutePathOrFileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this asset is a PNG image file.
    /// </summary>
    public bool IsPng => AbsolutePathOrFileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);

    public override string ToString() => PathOrFileName;
}
