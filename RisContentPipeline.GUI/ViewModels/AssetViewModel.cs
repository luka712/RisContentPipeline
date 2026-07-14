using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for an asset.
/// </summary>
public partial class AssetViewModel : ViewModelBase
{
    private readonly AssetsService _assetsService;
    private readonly ViewerService _viewerService;
    
    [ObservableProperty]
    private string _absoluteFilePath = string.Empty;
    
    [ObservableProperty]
    private string _buildPath = string.Empty;

    [ObservableProperty]
    private ImageViewModel? _image;
    
    [ObservableProperty]
    private PreferencesViewModel? _preferences;

    /// <summary>
    /// The constructor.
    /// </summary>
    public AssetViewModel()
    {
        _assetsService = App.Services.GetService<AssetsService>()!;
        _viewerService = App.Services.GetService<ViewerService>()!;
    }
    
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

    /// <summary>
    /// Removes this asset from the list.
    /// </summary>
    [RelayCommand]
    public void RemoveSelf()
    {
        _assetsService.RemoveAsset(this);
    }

    /// <summary>
    /// Opens the asset view.
    /// </summary>
    public void ViewSelf()
    {
        _viewerService.ViewAsset(this);
    }
}