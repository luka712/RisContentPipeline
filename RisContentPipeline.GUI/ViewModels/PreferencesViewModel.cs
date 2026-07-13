using System.ComponentModel;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Dto;
using RisContentPipeline.GUI.Services;
using SukiUI;
using SukiUI.Enums;
using SukiUI.Models;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for the preference window.
/// </summary>
public partial class PreferencesViewModel : ViewModelBase
{
    private const string DEFAULT_THEME = "Default";
    private const string DEFAULT_COLOR_THEME = "Blue";
    private const string PREFERENCES_FILE = "preferences.json";

    private readonly WindowsService _windowsService;
    
    [ObservableProperty] private string _buildDirectory = "Build";
    [ObservableProperty] private int _localServerPort;
    [ObservableProperty] private string _theme = DEFAULT_THEME;
    [ObservableProperty] private string _colorTheme = DEFAULT_COLOR_THEME;
    [ObservableProperty] private bool _nativeTitleBar;

    /// <summary>
    /// The KTX2 global settings.
    /// </summary>
    [ObservableProperty] private Ktx2SettingsViewModel _ktx2Settings = new();

    /// <summary>
    /// The constructor.
    /// </summary>
    public PreferencesViewModel()
    {
        _windowsService = App.Services.GetService<WindowsService>()!;
    }
    
    /// <summary>
    /// The available themes.
    /// </summary>
    public string[] Themes { get; } = ["Default", "Light", "Dark"];
    
    /// <summary>
    /// The available theme colors.
    /// </summary>
    public string[] ColorThemes { get; } = ["Blue", "Green", "Red", "Orange"];

    public Func<Task<string?>>? SelectFolderAsync { get; set; }
    public Action? CloseWindow { get; set; }


    [RelayCommand]
    private async Task BrowseBuildDirectoryAsync()
    {
        if (SelectFolderAsync == null) return;

        var folder = await SelectFolderAsync();
        if (folder != null)
        {
            BuildDirectory = folder;
        }
    }

    /// <summary>
    /// Save preferences and close the window.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        _ = SaveAsync();
        CloseWindow?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseWindow?.Invoke();
    }

    /// <summary>
    /// Loads preferences from the preference file.
    /// </summary>
    public static async Task<PreferencesViewModel> LoadAsync()
    {
        var viewModel = new PreferencesViewModel();
        if (!File.Exists(PREFERENCES_FILE))
        {
            return viewModel;
        }

        var json = await File.ReadAllTextAsync(PREFERENCES_FILE);
        var dto = JsonSerializer.Deserialize<PreferencesDto>(json);
        if (dto != null)
        {
            viewModel.BuildDirectory = dto.BuildDirectory;
            viewModel.LocalServerPort = dto.LocalServerPort;
            viewModel.Theme = dto.Theme ?? DEFAULT_THEME;
            viewModel.NativeTitleBar = dto.NativeTitleBar;
            viewModel.ColorTheme = dto.ColorTheme ?? DEFAULT_COLOR_THEME;
            viewModel.Ktx2Settings.EncodingTarget = dto.Ktx2Settings.EncodingTarget;
            viewModel.Ktx2Settings.GenerateMipmaps = dto.Ktx2Settings.GenerateMipmaps;
            viewModel.Ktx2Settings.QualityLevel = dto.Ktx2Settings.QualityLevel;
            viewModel.Ktx2Settings.FlipY = dto.Ktx2Settings.FlipY;
        }

        return viewModel;
    }

    /// <summary>
    /// Saves preferences to the preference file.
    /// </summary>
    public async Task SaveAsync()
    {
        var dto = new PreferencesDto()
        {
            BuildDirectory = BuildDirectory,
            LocalServerPort = LocalServerPort,
            Theme = Theme,
            ColorTheme = ColorTheme,
            NativeTitleBar = NativeTitleBar,
        };
        dto.Ktx2Settings.EncodingTarget = Ktx2Settings.EncodingTarget;
        dto.Ktx2Settings.GenerateMipmaps = Ktx2Settings.GenerateMipmaps;
        dto.Ktx2Settings.QualityLevel = Ktx2Settings.QualityLevel;
        dto.Ktx2Settings.FlipY = Ktx2Settings.FlipY;

        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(PREFERENCES_FILE, json);
    }

    /// <summary>
    /// Applies the current theme.
    /// </summary>
    public void ApplyTheme()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var theme = Theme switch
            {
                "Default" => ThemeVariant.Default,
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };

            var themeColor = ColorTheme switch
            {
                "Blue" => SukiColor.Blue,
                "Red" => SukiColor.Red,
                "Orange" => SukiColor.Orange,
                "Green" => SukiColor.Green,
                _ => SukiColor.Blue
            };
            
            SukiTheme.GetInstance().ChangeBaseTheme(theme);
            SukiTheme.GetInstance().ChangeColorTheme(themeColor);
        }, DispatcherPriority.Background);
    }
    

    /// <inheritdoc/>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Theme) || e.PropertyName == nameof(ColorTheme))
        {
            ApplyTheme();
        }

        if (e.PropertyName == nameof(NativeTitleBar))
        {
            _windowsService.NativeTitleBar = NativeTitleBar;
        }
    }
}