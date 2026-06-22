using System.Text.Json;
using System.Text.Json.Serialization;
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

    private static string _preferencesFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Constants.APP_DATA_FOLDER_NAME,
        PREFERENCES_FILE);

    /// <summary>
    /// The constructor.
    /// </summary>
    public Preferences()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        BuildDirectory = Path.Combine(appDataFolder, Constants.APP_DATA_FOLDER_NAME, "Build");
        
        LocalServerPort = 8787;
        Ktx2GlobalSettings = new ();
    }

    /// <summary>
    /// The directory where the build output is stored.
    /// </summary>
    [JsonPropertyName("build_directory")]
    public string BuildDirectory { get; set; }

    /// <summary>
    /// The port used for the local server.
    /// </summary>
    [JsonPropertyName("local_server_port")]
    public int LocalServerPort { get; set; }

    /// <summary>
    /// The global settings related to KTX2 texture conversion.
    /// </summary>
    [JsonPropertyName("ktx2_global_settings")]
    public Ktx2Settings Ktx2GlobalSettings { get; set; }
    
    /// <summary>
    /// Loads the preferences from the preference file.
    /// </summary>
    /// <returns>The <see cref="Preferences"/>.</returns>
    public static async Task<Preferences> LoadAsync()
    {
        if (!File.Exists(_preferencesFilePath))
        {
            return new Preferences();
        }

        string jsonContent = await File.ReadAllTextAsync(_preferencesFilePath);
        Preferences preferences = JsonSerializer.Deserialize<Preferences>(jsonContent)!;
        return preferences;
    }

    /// <summary>
    /// Saves the preferences to the preference file.
    /// </summary>
    public Task SaveAsync()
    {
        string jsonContent = JsonSerializer.Serialize(this);
        return File.WriteAllTextAsync(_preferencesFilePath, jsonContent);
    }
}