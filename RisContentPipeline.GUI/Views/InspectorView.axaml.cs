using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RisContentPipeline.GUI.Views;

public partial class InspectorView : UserControl
{
    private ContextMenu? _currentContextMenu;
    
    public InspectorView()
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
}