using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RisContentPipeline.GUI.ViewModels;
using RisContentPipeline.Ktx2;

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

    /// <summary>
    /// Invoked when the user clicks the copy button.
    /// </summary>
    private void MenuItem_CopySourcePath(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            var top = TopLevel.GetTopLevel(menuItem);
            if (menuItem.DataContext is QueuedPipelineItemViewModel vm)
            {
                var sourcePath = "";
                if (vm.Item.Item.Source is Ktx2PipelineSource source)
                {
                    sourcePath = source.FilePath;
                }
                else
                {
                    throw new NotImplementedException();
                }
                
                _ = top?.Clipboard?.SetTextAsync(sourcePath);
            }
        }
    }
    
    /// <summary>
    /// Invoked when the user clicks the copy button.
    /// </summary>
    private void MenuItem_CopyDestinationPath(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            var top = TopLevel.GetTopLevel(menuItem);
            if (menuItem.DataContext is QueuedPipelineItemViewModel vm)
            {
                var destPath = "";
                
                if (vm.Item.Item.Options is Ktx2PipelineOptions options)
                {
                    destPath = options.OutputPath;
                }
                else
                {
                    throw new NotImplementedException();
                }
                
                _ = top?.Clipboard?.SetTextAsync(destPath);
            }
        }
    }
}