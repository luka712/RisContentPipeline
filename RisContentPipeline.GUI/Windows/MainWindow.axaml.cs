using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.ViewModels;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Windows;

/// <summary>
/// The main window of the application.
/// </summary>
public partial class MainWindow : SukiWindow
{
    private readonly WindowsService _windowsService;
    
    public MainWindow()
    {
        _windowsService = App.Services.GetService<WindowsService>()!;
        _windowsService.AddWindow(this);
        
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (DataContext is MainViewModel vm)
        {
            // Wire up file picker services
            vm.OpenFilePicker = OpenFilePickerAsync;
            vm.OpenFolderPicker = OpenFolderPickerAsync;
            vm.ShowAboutWindow = ShowAboutDialog;
            vm.ShowImageViewer = ShowImageViewerDialog;

            await vm.InitializeAsync();
        }
    }


    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (DataContext is MainViewModel vm)
        {
            vm.SaveSession();
          //   _ = vm.SavePreferencesAsync();
        }
    }

    private async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return [];

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Files",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("Image Files") { Patterns = ["*.png"] },
                new FilePickerFileType("Data Files") { Patterns = ["*.json", "*.xml"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] }
            ]
        });

        return files;
    }

    private async Task<IStorageFolder?> OpenFolderPickerAsync()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Build Directory",
            AllowMultiple = false
        });

        return folders.FirstOrDefault();
    }

    private void ShowAboutDialog()
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }

    private void ShowPreferencesDialog()
    {
        if (DataContext is MainViewModel vm)
        {
            var preferencesWindow = new PreferencesWindow(vm.Preferences);
            preferencesWindow.ShowDialog(this);
        }
    }

    private void ShowImageViewerDialog(string filePath)
    {
        if (DataContext is MainViewModel vm)
        {
            // var viewerPath = ResolveViewerPath(vm.Preferences.ViewerPath);
            // var viewerWindow = new ImageViewerWindow(viewerPath, vm.Preferences.LocalServerPort);
            // viewerWindow.View(filePath);
        }
    }

    private static string ResolveViewerPath(string relativePath)
    {
        // Try from current working directory
        var fromCwd = Path.GetFullPath(relativePath);
        if (Directory.Exists(fromCwd)) return fromCwd;

        // Try from assembly directory
        var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (assemblyDir != null)
        {
            var fromAssembly = Path.Combine(assemblyDir, relativePath);
            if (Directory.Exists(fromAssembly)) return fromAssembly;
        }

        return relativePath;
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
