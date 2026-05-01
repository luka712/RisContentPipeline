using Python.Runtime;
using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.Scripting.Python;

/// <summary>
/// Provides C# integration with Python scripts for content pipeline processing.
/// </summary>
internal sealed class PythonIntegration : IDisposable
{
    private const string INTERNAL_SCRIPTS_DIRECTORY = "InternalScripts";
    private const string USER_SCRIPTS_DIRECTORY = "UserScripts";

    private readonly Context _context;
    private bool _initialized;
    private bool _disposed;
    private readonly PythonScriptingApi _api = new();

    /// <summary>
    /// The constructor takes a Context object which provides access to the content pipeline's build directory and other relevant information.
    /// </summary>
    /// <param name="context">The <see cref="Context"/>.</param>
    internal PythonIntegration(Context context)
    {
        _context = context;
    }

    /// <summary>
    /// Initializes the Python runtime. Must be called before using any Python functionality.
    /// </summary>
    internal void Initialize()
    {
        if (_initialized)
            return;

        if (_disposed)
            throw new ObjectDisposedException(nameof(PythonIntegration));

        try
        {
            // Find Python installation
            string pythonPath = FindPython();
            string pythonDllPath = GetPythonDllPath(pythonPath);

            if (!File.Exists(pythonDllPath))
            {
                throw new FileNotFoundException($"Python DLL not found at: {pythonDllPath}");
            }

            // Initialize Python runtime
            Runtime.PythonDLL = pythonDllPath;
            PythonEngine.Initialize();

            // Add the Python script directory to Python's path
            using (Py.GIL())
            {
                ImportModules();
            }

            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize Python runtime.", ex);
        }
    }

    private void ImportModules()
    {
        dynamic sys = Py.Import("sys");
        string scriptsDir = Path.GetFullPath(INTERNAL_SCRIPTS_DIRECTORY);
        if (!Directory.Exists(scriptsDir))
        {
            throw new InvalidOperationException($"Could not find '{INTERNAL_SCRIPTS_DIRECTORY}'.");
        }
        sys.path.append(scriptsDir);

        string userScriptsDir = Path.GetFullPath(USER_SCRIPTS_DIRECTORY);
        if (Directory.Exists(userScriptsDir))
        {
            sys.path.append(userScriptsDir);
        }
    }


    public void ProcessAsset(Script pythonScript, AssetFileOrFolder fileOrFolder)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("Python runtime not initialized. Call Initialize() first.");
        }

        if (_disposed)
            throw new ObjectDisposedException(nameof(PythonIntegration));

        _api.current_asset = new Data.PythonAssetFile(fileOrFolder);

        Directory.CreateDirectory(_context.BuildDirectory);
        var scriptName = pythonScript.FilePath;   

        try
        {
            using (Py.GIL())
            {
                ImportModules();
                scriptName = scriptName.Split('/').Last().Split('\\').Last().Replace(".py", "");;
                dynamic script = Py.Import(scriptName);
                script.api = _api;
                dynamic builtins = Py.Import("builtins");


                dynamic pyResult = script.process_asset();

                if (!pyResult["success"])
                {
                    _context.BuildLogger.Success($"Error during '{script}' processing: {pyResult["error"]}");
                }

                if (_api.current_asset.IsDirty)
                {
                    // If the content was modified, save it back to the original file
                    var modifiedContent = _api.current_asset.content;
                    if (modifiedContent != null)
                    {
                        File.WriteAllText(fileOrFolder.AbsolutePathOrFileName, modifiedContent);
                        _context.BuildLogger.Success($"Modified content saved back to '{fileOrFolder.AbsolutePathOrFileName}'.");
                    }
                }
            }
        }
        catch (PythonException ex)
        {
            throw new Exception($"Python module '{scriptName}' error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Disposes the Python runtime.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_initialized)
        {
            try
            {
                PythonEngine.Shutdown();
            }
            catch
            {
                // Ignore errors during shutdown
            }
            _initialized = false;
        }

        _disposed = true;
    }

    /// <summary>
    /// Finds the Python executable path.
    /// </summary>
    private string FindPython()
    {
        var path = "";
        if (OperatingSystem.IsWindows())
        {
            path += "python-3.11.8";
        }
        else
        {
            throw new PlatformNotSupportedException("Only Windows is supported for Python integration in this implementation.");
        }

        return path;
    }

    /// <summary>
    /// Gets the Python DLL path from the Python executable path.
    /// </summary>
    private string GetPythonDllPath(string pythonExePath)
    {
        if (!Directory.Exists(pythonExePath))
        {
            throw new DirectoryNotFoundException($"Python directory not found: {pythonExePath}");
        }

        if (OperatingSystem.IsWindows())
        {
            // Try to find python3XX.dll in the Python directory (exclude python3.dll stub)
            var dllFiles = Directory.GetFiles(pythonExePath, "python3*.dll")
                .Where(f => !f.EndsWith("python3.dll", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(f => f) // Prefer higher versions
                .ToArray();

            if (dllFiles.Length > 0)
            {
                return dllFiles[0];
            }

            throw new FileNotFoundException($"Could not locate Python versioned DLL (pythonXXX.dll) in: {pythonExePath}");
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux uses libpython3.XX.so with version number
            var soFiles = Directory.GetFiles(Path.Combine(pythonExePath, "..", "lib"), "libpython3.*.so")
                .OrderByDescending(f => f)
                .ToArray();

            if (soFiles.Length > 0)
                return soFiles[0];

            throw new FileNotFoundException("Could not locate libpython3.XX.so");
        }
        else if (OperatingSystem.IsMacOS())
        {
            // macOS uses libpython3.XX.dylib with version number
            var dylibFiles = Directory.GetFiles(Path.Combine(pythonExePath, "..", "lib"), "libpython3.*.dylib")
                .OrderByDescending(f => f)
                .ToArray();

            if (dylibFiles.Length > 0)
                return dylibFiles[0];

            throw new FileNotFoundException("Could not locate libpython3.XX.dylib");
        }

        throw new PlatformNotSupportedException("Unsupported operating system for Python integration.");
    }
}