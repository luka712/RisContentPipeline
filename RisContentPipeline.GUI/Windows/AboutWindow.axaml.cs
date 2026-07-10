using Avalonia.Interactivity;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Windows;

public partial class AboutWindow : SukiWindow
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
