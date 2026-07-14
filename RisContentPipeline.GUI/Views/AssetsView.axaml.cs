using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using RisContentPipeline.GUI.ViewModels;

namespace RisContentPipeline.GUI.Views;

public partial class AssetsView : UserControl
{
    private ContextMenu? _currentContextMenu;

    public AssetsView()
    {
        InitializeComponent();
    }

    private void MenuBase_OnOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu contextMenu && contextMenu != _currentContextMenu)
        {
            _currentContextMenu?.Close();
            _currentContextMenu = contextMenu;
        }
    }

    /// <summary>
    /// Invoked when the user clicks the close button.
    /// </summary>
    private void MenuItem_CloseClick(object? sender, RoutedEventArgs e)
    {
        _currentContextMenu?.Close();
        _currentContextMenu = null;
    }

    /// <summary>
    /// Invoked when the user clicks the copy button.
    /// </summary>
    private void MenuItem_ClipboardClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            var top = TopLevel.GetTopLevel(menuItem);
            if (menuItem.DataContext is AssetViewModel vm)
            {
                _ = top?.Clipboard?.SetTextAsync(vm.AbsoluteFilePath);
            }
        }
    }
}