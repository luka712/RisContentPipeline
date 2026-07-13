using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.ViewModels;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Windows;

public partial class PreferencesWindow : SukiWindow
{
    private readonly WindowsService _windowsService;
    
    /// <summary>
    /// The constructor for the PreferencesWindow.
    /// </summary>
    /// <param name="viewModel">The <see cref="PreferencesViewModel"/>.</param>
    public PreferencesWindow(PreferencesViewModel viewModel) 
    {
        DataContext = viewModel;
        viewModel.SelectFolderAsync = SelectFolderAsync;
        viewModel.CloseWindow = () => Close();

        _windowsService = App.Services.GetService<WindowsService>()!;
        _windowsService.AddWindow(this);
        
        InitializeComponent();
    }

    private async Task<string?> SelectFolderAsync()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Build Directory",
            AllowMultiple = false
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }
}
