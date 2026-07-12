using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RisContentPipeline.GUI.Models;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for the preferences window.
/// </summary>
public partial class PreferencesViewModel : ViewModelBase
{
    private readonly Preferences _preferences;
    
    [ObservableProperty] private string _buildDirectory;
    [ObservableProperty] private Ktx2EncodingTarget _encodingTarget;
    [ObservableProperty] private bool _generateMipmaps;
    [ObservableProperty] private bool _flipY;
    [ObservableProperty] private int _qualityLevel;
    [ObservableProperty] private int _localServerPort;

    public Func<Task<string?>>? SelectFolderAsync { get; set; }
    public Action? CloseWindow { get; set; }

    public PreferencesViewModel() : this(new Preferences()) { }

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="preferences">The <see cref="Preferences"/>.</param>
    public PreferencesViewModel(Preferences preferences)
    {
        _preferences = preferences;
        
        // Load current values
        BuildDirectory = preferences.BuildDirectory;
        EncodingTarget= preferences.Ktx2GlobalSettings.EncodeTarget;
        GenerateMipmaps = preferences.Ktx2GlobalSettings.GenerateMipmaps;
        FlipY = preferences.Ktx2GlobalSettings.FlipY;
        QualityLevel = preferences.Ktx2GlobalSettings.QualityLevel;
        LocalServerPort = preferences.LocalServerPort;
    }

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

    [RelayCommand]
    private void Save()
    {
        // Apply changes to preferences
        _preferences.BuildDirectory = BuildDirectory;
        _preferences.Ktx2GlobalSettings.EncodeTarget = EncodingTarget;
        _preferences.Ktx2GlobalSettings.GenerateMipmaps = GenerateMipmaps;
        _preferences.Ktx2GlobalSettings.FlipY = FlipY;
        _preferences.Ktx2GlobalSettings.QualityLevel = QualityLevel;
        _preferences.LocalServerPort = LocalServerPort;
        
        CloseWindow?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseWindow?.Invoke();
    }
}
