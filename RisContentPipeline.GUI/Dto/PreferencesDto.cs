namespace RisContentPipeline.GUI.Dto;

/// <summary>
/// The preferences DTO.
/// </summary>
public record PreferencesDto
{
    /// <summary>
    /// The build directory.
    /// </summary>
    public required string BuildDirectory { get; set; }
    
    /// <summary>
    /// The local server port.
    /// </summary>
    public int LocalServerPort { get; set; }
    
    /// <summary>
    /// The theme to use.
    /// </summary>
    public string? Theme { get; set; } 
    
    /// <summary>
    /// The color theme to use.
    /// </summary>
    public string? ColorTheme { get; set; }
    
    /// <summary>
    /// Shows the native title bar instead of the custom one.
    /// </summary>
    public bool NativeTitleBar { get; set; }
    
    /// <summary>
    /// The KTX2 settings.
    /// </summary>
    public Ktx2SettingsDto Ktx2Settings { get; init; } = new();
}