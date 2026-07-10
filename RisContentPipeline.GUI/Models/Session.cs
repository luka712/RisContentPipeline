namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Session data that is persisted between application runs.
/// </summary>
public class Session
{
    /// <summary>
    /// List of build script file paths.
    /// </summary>
    public List<string> BuildScripts { get; set; } = [];
}
