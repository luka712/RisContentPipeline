using Eto.Drawing;

namespace RisContentPipeline.GUI;

/// <summary>
/// The application resources.
/// </summary>
public class Constants
{
    /// <summary>
    /// The name of the application.
    /// </summary>
    public const string APP_NAME = "Ris Content Pipeline";
    
    /// <summary>
    /// The path to the application icon.
    /// </summary>
    public const string ICO_ICON_FILE_PATH = "Resources/Icon.ico";
    
    /// <summary>
    /// The path to the application icon.
    /// </summary>
    public const string PNG_ICON_FILE_PATH = "Resources/Icon.png";

    /// <summary>
    /// The main application icon, loaded from the specified file path.
    /// </summary>
    public static Icon MAIN_ICON = new Icon(ICO_ICON_FILE_PATH);
}