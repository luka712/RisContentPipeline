namespace RisContentPipeline.GUI.Models;

/// <summary>
/// Represents a Python script that can be executed during the build process.
/// </summary>
public class Script
{
    /// <summary>
    /// Creates a new script from the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the Python script file.</param>
    public Script(string filePath)
    {
        FilePath = filePath;
        Name = Path.GetFileNameWithoutExtension(filePath);
    }

    /// <summary>
    /// The full path to the script file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// The display name of the script (file name without extension).
    /// </summary>
    public string Name { get; }

    public override string ToString() => Name;
}
