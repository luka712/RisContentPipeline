using System.Text.Json;
using RisContentPipeline.GUI.Dto;
using RisContentPipeline.GUI.ViewModels;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// Service for storing user preferences.
/// </summary>
public class UserPreferencesService
{
    private const string PREFERENCES_FILE = "preferences.json";
    
    private readonly MessageService _messageService;
    private readonly string _preferencesFilePath;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="messageService">The <see cref="MessageService"/>.</param>
    /// <param name="foldersService">The <see cref="FoldersService"/>.</param>
    public UserPreferencesService(MessageService messageService, FoldersService foldersService)
    {
        _messageService = messageService;
        _preferencesFilePath = Path.Combine(foldersService.GetApplicationDataPath(), PREFERENCES_FILE);
    }
    
    /// <summary>
    /// The reference to the main view model.
    /// </summary>
    public MainViewModel ViewModel { get; set; } = null!;

    /// <summary>
    /// The local server port.
    /// </summary>
    public int LocalServerPort => ViewModel.Preferences.LocalServerPort;
    
    /// <summary>
    /// Saves preferences to the preference file.
    /// </summary>
    public async Task SaveAsync(PreferencesViewModel viewModel)
    {
        var dto = new PreferencesDto()
        {
            BuildDirectory = viewModel.BuildDirectory,
            LocalServerPort = viewModel.LocalServerPort,
            Theme = viewModel.Theme,
            ColorTheme = viewModel.ColorTheme,
            NativeTitleBar = viewModel.NativeTitleBar,
        };
        dto.Ktx2Settings.EncodingTarget = viewModel.Ktx2Settings.EncodingTarget;
        dto.Ktx2Settings.GenerateMipmaps = viewModel.Ktx2Settings.GenerateMipmaps;
        dto.Ktx2Settings.QualityLevel = viewModel.Ktx2Settings.QualityLevel;
        dto.Ktx2Settings.FlipY = viewModel.Ktx2Settings.FlipY;
        
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_preferencesFilePath, json);
    }
    
    /// <summary>
    /// Loads preferences from the preference file.
    /// </summary>
    public async Task<PreferencesViewModel> LoadAsync()
    {
        var viewModel = new PreferencesViewModel();
        
        if (!File.Exists(_preferencesFilePath))
        {
            return viewModel;
        }

        PreferencesDto? dto = null;

        try
        {
           var  json = await File.ReadAllTextAsync(_preferencesFilePath);
           dto = JsonSerializer.Deserialize<PreferencesDto>(json);
        }
        catch(Exception ex)
        {
            _messageService.Error($"Failed to load preferences. Exception: {ex.Message}");           
            return viewModel;
        }

        if (dto != null)
        {
            viewModel.BuildDirectory = dto.BuildDirectory;
            viewModel.LocalServerPort = dto.LocalServerPort;
            viewModel.NativeTitleBar = dto.NativeTitleBar;
            
            if (!String.IsNullOrEmpty(dto.Theme))
            {
                viewModel.Theme = dto.Theme;
            }

            if (!String.IsNullOrEmpty(dto.ColorTheme))
            {
                viewModel.ColorTheme = dto.ColorTheme;
            }
            viewModel.Ktx2Settings.EncodingTarget = dto.Ktx2Settings.EncodingTarget;
            viewModel.Ktx2Settings.GenerateMipmaps = dto.Ktx2Settings.GenerateMipmaps;
            viewModel.Ktx2Settings.QualityLevel = dto.Ktx2Settings.QualityLevel;
            viewModel.Ktx2Settings.FlipY = dto.Ktx2Settings.FlipY;
        }

        return viewModel;
    }
}