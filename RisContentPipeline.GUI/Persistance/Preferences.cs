using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Persistance;

/// <summary>
/// The preferences for the application.
/// </summary>
public class Preferences
{
    /// <summary>
    /// The port used for the local server.
    /// </summary>
    public int LocalServerPort { get; set; } = 8787;
    
    /// <summary>
    /// The global settings related to KTX2 texture conversion.
    /// </summary>
    public Ktx2Settings Ktx2GlobalSettings { get; set; } = new();
}