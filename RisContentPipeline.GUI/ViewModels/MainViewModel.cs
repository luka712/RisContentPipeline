using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Models;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Windows;
using SukiUI.Controls;
using SukiUI.Toasts;

namespace RisContentPipeline.GUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AssetsService _assetsService;
    private readonly UserPreferencesService _preferencesService;
    private readonly MessageService _messageService;

    private readonly PipelineContext _context;

    /// <summary>
    /// The currently selected asset in UI.
    /// </summary>
    [ObservableProperty] private AssetViewModel? _selectedAsset;

    [ObservableProperty] private Script? _selectedScript;
    [ObservableProperty] private bool _isBuilding;
    [ObservableProperty] private string _statusText = "Ready";
    
    [ObservableProperty]
    private ObservableCollection<LogMessageViewModel> _messages = new();

    [ObservableProperty]
    private ObservableCollection<QueuedPipelineItemViewModel> _queuedItems = [];

    /// <summary>
    /// The currently added assets.
    /// </summary>
    public ObservableCollection<AssetViewModel> Assets => _assetsService.Assets;

    /// <summary>
    /// The preferences view model.
    /// </summary>
    public PreferencesViewModel Preferences { get; private set; }

    // File picker service - set by the view
    public Func<Task<IReadOnlyList<IStorageFile>>>? OpenFilePicker { get; set; }
    public Func<Task<IStorageFolder?>>? OpenFolderPicker { get; set; }
    public Action? ShowAboutWindow { get; set; }

    /// <summary>
    /// The constructor.
    /// </summary>
    public MainViewModel()
    {
        _assetsService = App.Services.GetService<AssetsService>()!;
        _preferencesService = App.Services.GetService<UserPreferencesService>()!;
        _messageService = App.Services.GetService<MessageService>()!;
        
        _assetsService.OnItemQueued += item => QueuedItems.Add(new QueuedPipelineItemViewModel(item));
        _assetsService.OnBuildStarted += () => QueuedItems.Clear();

        _messageService.OnMessage += msg => Messages.Add(msg);
        _messageService.OnClear += () => Messages.Clear();
    }

    /// <summary>
    /// The toast manager.
    /// </summary>
    public ISukiToastManager ToastManager { get; } = new SukiToastManager();

    /// <summary>
    /// Displays a toast message.
    /// </summary>
    public void DisplayToast(string title, string content, int duration = 3000)
    {
        ToastManager.CreateToast()
            .WithTitle(title).WithContent(content)
            .Dismiss().After(TimeSpan.FromMilliseconds(duration))
            .Dismiss().ByClicking()
            .Queue();
    }

    // private void ShowUpdatingToast()
    // {
    //     var progress = new ProgressBar() { Value = 0, ShowProgressText = true };
    //     var toast = ToastManager.CreateToast()
    //         .WithTitle("Updating...")
    //         .WithContent(progress)
    //         .Queue();
    //     var timer = new Timer(20);
    //     timer.Elapsed += (_, _) =>
    //     {
    //         Dispatcher.UIThread.Invoke(() =>
    //         {
    //             progress.Value += 1;
    //             if (progress.Value < 100) return;
    //             timer.Dispose();
    //             ToastManager.Dismiss(toast);
    //         });
    //     };
    //     timer.Start();
    // }

    public async Task InitializeAsync()
    {
        Preferences = await _preferencesService.LoadAsync();
        App.Services.GetService<LocalWebServer>()!.Start();
        _assetsService.PreferencesViewModel = Preferences;
        Preferences.ApplyTheme();
    }

    [RelayCommand]
    private async Task AddFilesAsync()
    {
        if (OpenFilePicker == null) return;

        var files = await OpenFilePicker();
        foreach (var file in files)
        {
            if (file.Path.LocalPath is { } path)
            {
                _assetsService.AddFile(path);
            }
        }
    }

    [RelayCommand]
    private void RemoveSelectedAsset()
    {
        if (SelectedAsset != null)
        {
            var index = Assets.IndexOf(SelectedAsset);
            _assetsService.RemoveAsset(index);
            SelectedAsset = null;
        }
    }

    [RelayCommand]
    private void ClearAssets()
    {
        _assetsService.Assets.Clear();
        SelectedAsset = null;
    }

    [RelayCommand]
    private async Task BuildAsync()
    {
        if (IsBuilding) return;

        try
        {
            await _assetsService.BuildAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Build failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearMessages()
    {
        _messageService.Clear();
    }

    [RelayCommand]
    private void ShowAbout()
    {
        ShowAboutWindow?.Invoke();
    }

    [RelayCommand]
    private void ShowPreferencesWindow(SukiWindow owner)
    {
        var preferencesWindow = new PreferencesWindow(Preferences);
        preferencesWindow.Show(owner);
    }

    [RelayCommand]
    private async Task SetBuildDirectoryAsync()
    {
        // if (OpenFolderPicker == null) return;
        //
        // var folder = await OpenFolderPicker();
        // if (folder?.Path.LocalPath is { } path)
        // {
        //     _context.Preferences.BuildDirectory = path;
        //     OnPropertyChanged(nameof(Preferences));
        // }
    }

    public void SaveSession() => _context.SaveSession();
    //public Task SavePreferencesAsync() => _context.SavePreferencesAsync();

    public void Dispose()
    {
        _context.SaveSession();
        _context.Dispose();
    }
}