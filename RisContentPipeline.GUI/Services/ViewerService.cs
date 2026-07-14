using Avalonia.Controls;
using RisContentPipeline.GUI.ViewModels;
using RisContentPipeline.GUI.Views;
using RisContentPipeline.GUI.Windows;
using ImageViewerWindow = RisContentPipeline.GUI.Windows.ImageViewerWindow;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// The viewer service.
/// </summary>
public class ViewerService
{
    private readonly WindowsService _windowsService;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="windowsService">The <see cref="WindowsService"/>.</param>
    public ViewerService(WindowsService windowsService)
    {
        _windowsService = windowsService;
    }
    
    /// <summary>
    /// Views the specified asset.
    /// </summary>
    /// <param name="asset">The <see cref="AssetViewModel"/>.</param>
    public void ViewAsset(AssetViewModel asset)
    {
        if (asset.IsImage)
        {
            ImageViewerWindow window = new ImageViewerWindow();
            window.DataContext = asset;
            _windowsService.ApplySettings(window);
            window.Show();
        }
    }
}