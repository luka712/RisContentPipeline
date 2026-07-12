using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RisContentPipeline.GUI.Models;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Windows;
using SukiUI.Controls;
using SukiUI.Toasts;

namespace RisContentPipeline.GUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly PipelineContext _context;

    /// <summary>
    /// The currently selected asset in UI.
    /// </summary>
    [ObservableProperty] private AssetViewModel? _selectedAsset;

    [ObservableProperty] private Script? _selectedScript;
    [ObservableProperty] private bool _isBuilding;
    [ObservableProperty] private string _statusText = "Ready";

    [ObservableProperty]
    private ObservableCollection<LogMessage> _messages =
        [new LogMessage(MessageLogLevel.ERROR, "No messages", DateTime.Now)];

    public ObservableCollection<AssetViewModel> Assets => _context.Assets;
    


    /// <summary>
    /// The preferences view model.
    /// </summary>
    public PreferencesViewModel Preferences { get; }

    // File picker service - set by the view
    public Func<Task<IReadOnlyList<IStorageFile>>>? OpenFilePicker { get; set; }
    public Func<Task<IStorageFolder?>>? OpenFolderPicker { get; set; }
    public Action? ShowAboutWindow { get; set; }
    public Action<string>? ShowImageViewer { get; set; }

    public MainViewModel() : this(new PipelineContext())
    {
    }

    public MainViewModel(PipelineContext context)
    {
        _context = context;
        _context.OnBuildStarted += () =>
        {
            IsBuilding = true;
            StatusText = "Building...";
        };
        _context.OnBuildFinished += () =>
        {
            IsBuilding = false;
            StatusText = "Build completed";
        };

        Preferences = new PreferencesViewModel(context.Preferences);
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
        await _context.LoadPreferencesAsync();
        _context.LoadSession();
    }

    [RelayCommand]
    private async Task AddFilesAsync()
    {
        if (OpenFilePicker == null) return;

        var files = await OpenFilePicker();
        foreach (var file in files)
        {
            if (file.Path.LocalPath is { } path)
                _context.AddFile(path);
        }
    }

    [RelayCommand]
    private void RemoveSelectedAsset()
    {
        if (SelectedAsset != null)
        {
            var index = Assets.IndexOf(SelectedAsset);
            _context.RemoveAsset(index);
            SelectedAsset = null;
        }
    }

    [RelayCommand]
    private void ClearAssets()
    {
        _context.Assets.Clear();
        SelectedAsset = null;
    }

    [RelayCommand]
    private async Task BuildAsync()
    {
        if (IsBuilding) return;

        try
        {
            await _context.BuildAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Build failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearMessages()
    {
        _context.ClearMessages();
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
    private void ViewSelectedAsset()
    {
        if (SelectedAsset?.AbsoluteFilePath is { } path &&
            (SelectedAsset.IsImage || path.EndsWith(".ktx2", StringComparison.OrdinalIgnoreCase)))
        {
            ShowImageViewer?.Invoke(path);
        }
    }

    [RelayCommand]
    private async Task SetBuildDirectoryAsync()
    {
        if (OpenFolderPicker == null) return;

        var folder = await OpenFolderPicker();
        if (folder?.Path.LocalPath is { } path)
        {
            _context.Preferences.BuildDirectory = path;
            OnPropertyChanged(nameof(Preferences));
        }
    }

    public void SaveSession() => _context.SaveSession();
    public Task SavePreferencesAsync() => _context.SavePreferencesAsync();

    public void Dispose()
    {
        _context.SaveSession();
        _context.Dispose();
    }
}