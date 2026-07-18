using System.Reflection;

namespace RisContentPipeline.GUI.Services;

public class FoldersService
{
    private readonly AppConfig _config;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="config">The <see cref="AppConfig"/>.</param>
    public FoldersService(AppConfig config)
    {
        _config = config;
    }

    private string GetExecutablePath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
    
    /// <summary>
    /// The path to the application data directory.
    /// Best folder to store application data such as settings, user data, etc.
    /// 1. If the application is self-contained, the path is the location of the executable.
    /// 2. Otherwise, the path is the path to the "RisContentPipeline" folder in the application data directory.
    /// </summary>
    /// <returns>The file path.</returns>
    public string GetApplicationDataPath()
    {
        if (_config.IsSelfContained)
        {
            return GetExecutablePath();
        }
        
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APP_NAME);
    }
    
    /// <summary>
    /// If the application is self-contained, the path is the location of the executable.
    /// Otherwise, the path is the path to the "RisContentPipeline" folder in the My Documents directory.
    /// </summary>
    /// <param name="subFolder">The optional sub-folder to add.</param>
    /// <returns>The file path.</returns>
    public string GetMyDocumentsPath(string subFolder = "")
    {
        if (_config.IsSelfContained)
        {
            return Path.Combine(GetExecutablePath(), subFolder);
        }
        
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.APP_NAME, subFolder);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return directory;
    }
}