using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Windows;

/// <summary>
/// The about window.
/// </summary>
public partial class AboutWindow : SukiWindow
{
    public AboutWindow()
    {
        InitializeComponent();
        
        var windowsService = App.Services.GetService<WindowsService>()!;
        windowsService.AddWindow(this);
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_MailToClipboard(object? sender, RoutedEventArgs e)
    {
        _ = Clipboard.SetTextAsync("erkapic.luka.dev@gmail.com");
    }
}
