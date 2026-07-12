using CommunityToolkit.Mvvm.ComponentModel;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for an asset.
/// </summary>
public partial class AssetViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _absoluteFilePath = string.Empty;
    
    [ObservableProperty]
    private string _buildPath = string.Empty;

    [ObservableProperty]
    private ImageViewModel? _image;

    /// <summary>
    /// Gets the file name or path without the directory.
    /// </summary>
    public string FileName => Path.GetFileName(AbsoluteFilePath);

    
    /// <summary>
    /// Gets whether this asset is a JSON file.
    /// </summary>
    public bool IsJson => AbsoluteFilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this asset is an XML file.
    /// </summary>
    public bool IsXml => AbsoluteFilePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this asset is an image file.
    /// </summary>
    public bool IsImage => AbsoluteFilePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase);

    public override string ToString() => FileName;
}