using System.Text.Json;
using RisContentPipeline.GUI.ViewModels;

namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Application preferences that are persisted between sessions.
/// </summary>
public class Preferences
{
    private const string PREFERENCES_FILE = "preferences.json";

    /// <summary>
    /// The directory where built assets are output.
    /// </summary>
    public string BuildDirectory { get; set; } = "Build";

    /// <summary>
    /// The port for the local web server used by the image viewer.
    /// </summary>
    public int LocalServerPort { get; set; } = 5050;

    /// <summary>
    /// The path to the browser viewer files.
    /// </summary>
    public string ViewerPath { get; set; } = "BrowserViewer";

    /// <summary>
    /// Global KTX2 settings applied to new images.
    /// </summary>
    public Ktx2SettingsViewModel Ktx2GlobalSettings { get; set; } = new();

    /// <summary>
    /// Loads preferences from the preference file.
    /// </summary>
    public static async Task<Preferences> LoadAsync()
    {
        if (!File.Exists(PREFERENCES_FILE))
            return new Preferences();

        var json = await File.ReadAllTextAsync(PREFERENCES_FILE);
        return JsonSerializer.Deserialize<Preferences>(json) ?? new Preferences();
    }

    /// <summary>
    /// Saves preferences to the preference file.
    /// </summary>
    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(PREFERENCES_FILE, json);
    }
}
