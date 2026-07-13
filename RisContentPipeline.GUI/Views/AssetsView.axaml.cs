using Avalonia.Controls;
using Avalonia.Interactivity;

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
        if (sender is ContextMenu contextMenu)
        {
            _currentContextMenu?.Close();
            _currentContextMenu = contextMenu;
        }
    }
    
    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        _currentContextMenu?.Close();
        _currentContextMenu = null;
    }
}