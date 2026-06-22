using System.Text.Json;
using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Model;

/// <summary>
/// The preferences for the application.
/// </summary>
public class Preferences
{
    /// <summary>
    /// The name of the JSON file used to persist the preferences between application runs.
    /// </summary>
    internal const string PREFERENCES_FILE = "preferences.json";

    private static string? _preferencesFilePath;

    /// <summary>
    /// The constructor.
    /// </summary>
    public Preferences()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        BuildDirectory = Path.Combine(appDataFolder, Constants.APP_DATA_FOLDER_NAME, "Build");
    }

    /// <summary>
    /// The directory where the build output is stored.
    /// </summary>
    public string BuildDirectory { get; set; }

    /// <summary>
    /// The port used for the local server.
    /// </summary>
    public int LocalServerPort { get; set; } = 8787;

    /// <summary>
    /// The global settings related to KTX2 texture conversion.
    /// </summary>
    public Ktx2Settings Ktx2GlobalSettings { get; set; } = new();


    /// <summary>
    /// Gets the path to the preference file.
    /// </summary>
    /// <returns>The file path to the preference file.</returns>
    private static string GetPreferencesFilePath()
    {
        if (_preferencesFilePath is null)
        {
            _preferencesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.APP_DATA_FOLDER_NAME, PREFERENCES_FILE);
        }

        return _preferencesFilePath;
    }

    /// <summary>
    /// Loads the preferences from the preference file.
    /// </summary>
    /// <returns>The <see cref="Preferences"/>.</returns>
    public static async Task<Preferences> LoadAsync()
    {
        var filePath = GetPreferencesFilePath();
        if (!File.Exists(filePath))
        {
            return new Preferences();
        }

        string jsonContent = await File.ReadAllTextAsync(PREFERENCES_FILE);
        Preferences preferences = JsonSerializer.Deserialize<Preferences>(jsonContent)!;
        return preferences;
    }

    /// <summary>
    /// Saves the preferences to the preference file.
    /// </summary>
    public Task SaveAsync()
    {
        string jsonContent = JsonSerializer.Serialize(this);
        var filePath = GetPreferencesFilePath();
        return File.WriteAllTextAsync(filePath, jsonContent);
    }
}