using Eto.Forms;
using RisContentPipeline.GUI.Settings;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    private static string _preferencesFilePath;

    /// <summary>
    /// The constructor.
    /// </summary>
    public Preferences()
    {
        BuildDirectory = GetDefaultBuildDirectory();
        _preferencesFilePath = GetPreferencesFilePath();

        LocalServerPort = 8787;
        Ktx2GlobalSettings = new();
    }

    /// <summary>
    /// Gets the directory that will act as default build directory for the application. If the directory does not exist, it will be created.
    /// If the application is running in portable mode, the build directory will be created in the same directory as the application.
    /// Otherwise, it will be created in the user's application data folder.
    /// </summary>
    /// <returns>The build directory</returns>
    private string GetDefaultBuildDirectory()
    {
        var directory = Constants.PORTABLE_MODE ? AppContext.BaseDirectory : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!Constants.PORTABLE_MODE)
        {
            directory = Path.Combine(directory, Constants.APP_DATA_FOLDER_NAME);
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        directory = Path.Combine(directory, "Build");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return directory;
    }

    /// <summary>
    /// Gets the path to the preferences file. 
    /// If the application is running in portable mode, the preferences file will be created in the same directory as the application.
    /// If the application is not running in portable mode, the preferences file will be created in the user's application data folder.
    /// </summary>
    /// <returns>The file path to the preferences file.</returns>
    private string GetPreferencesFilePath()
    {
        var directory = Constants.PORTABLE_MODE ? AppContext.BaseDirectory : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!Constants.PORTABLE_MODE)
        {
            directory = Path.Combine(directory, Constants.APP_DATA_FOLDER_NAME);
        }
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return Path.Combine(directory, PREFERENCES_FILE);
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