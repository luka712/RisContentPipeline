using System.Reflection;
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

    private readonly string _currentDirectory;
    private readonly Context _context;
    private bool _initialized;
    private bool _disposed;
    private readonly PythonScriptingApi _api;

    /// <summary>
    /// The constructor takes a Context object which provides access to the content pipeline's build directory and other relevant information.
    /// </summary>
    /// <param name="context">The <see cref="Context"/>.</param>
    internal PythonIntegration(Context context)
    {
        _context = context;
        _api = new(_context.PipelineSystem);
        _currentDirectory = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location
        )!;
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
            scriptsDir = Path.Combine(_currentDirectory, INTERNAL_SCRIPTS_DIRECTORY);
            if (!Directory.Exists(scriptsDir))
            {
                throw new InvalidOperationException($"Could not find '{INTERNAL_SCRIPTS_DIRECTORY}'.");
            }
        }

        sys.path.append(scriptsDir);

        string userScriptsDir = Path.GetFullPath(USER_SCRIPTS_DIRECTORY);
        if (Directory.Exists(userScriptsDir))
        {
            sys.path.append(userScriptsDir);
        }
    }

    private void AssertInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("Python runtime not initialized. Call Initialize() first.");
        }

        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PythonIntegration));
        }
    }

<<<<<<< HEAD
    // TODO: add doc comment
    public void BeforeBuild(Script pythonScript)
    {
=======
    /// <summary>
    /// Invokes the optional <c>before_build</c> function of the supplied Python script.
    /// Used to perform set-up work or to register additional pipelines on the
    /// <see cref="IPipelineSystem"/> before the build runs.
    /// If the script does not define a <c>before_build</c> function this method is a no-op.
    /// </summary>
    /// <param name="pythonScript">The user-supplied <see cref="Script"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown when the Python runtime has not been initialized.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    /// <exception cref="Exception">Thrown wrapping any <see cref="PythonException"/> raised by the script.</exception>
    public void BeforeBuild(Script pythonScript)
    {
        InvokeOptionalScriptHook(pythonScript, "before_build");
    }

    /// <summary>
    /// Invokes the optional <c>after_build</c> function of the supplied Python script.
    /// Used to perform clean-up or post-processing tasks after the build has completed.
    /// If the script does not define an <c>after_build</c> function this method is a no-op.
    /// </summary>
    /// <param name="pythonScript">The user-supplied <see cref="Script"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown when the Python runtime has not been initialized.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
    /// <exception cref="Exception">Thrown wrapping any <see cref="PythonException"/> raised by the script.</exception>
    public void AfterBuild(Script pythonScript)
    {
        InvokeOptionalScriptHook(pythonScript, "after_build");
    }

    /// <summary>
    /// Imports the script module and invokes the named hook function if it exists.
    /// </summary>
    private void InvokeOptionalScriptHook(Script pythonScript, string hookName)
    {
>>>>>>> origin/main
        AssertInitialized();

        var scriptName = pythonScript.FilePath;

        try
        {
            using (Py.GIL())
            {
                ImportModules();
<<<<<<< HEAD
                scriptName = scriptName.Split('/').Last().Split('\\').Last().Replace(".py", "");
                dynamic script = Py.Import(scriptName);
                script.api = _api;
                dynamic builtins = Py.Import("builtins");
                
                if (script.HasAttr("before_build"))
                {
                    script.before_build();
=======
                scriptName = Path.GetFileNameWithoutExtension(scriptName);
                dynamic script = Py.Import(scriptName);
                script.api = _api;

                if (script.HasAttr(hookName))
                {
                    script.GetAttr(hookName).Invoke();
>>>>>>> origin/main
                }
            }
        }
        catch (PythonException ex)
        {
<<<<<<< HEAD
            throw new Exception($"Python module '{scriptName}' error: {ex.Message}", ex);
        }
    }
    
    // TODO: create AfterBuild method
=======
            throw new Exception($"Python module '{scriptName}' error in '{hookName}': {ex.Message}", ex);
        }
    }
>>>>>>> origin/main

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
                scriptName = scriptName.Split('/').Last().Split('\\').Last().Replace(".py", "");
                ;
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
                        _context.BuildLogger.Success(
                            $"Modified content saved back to '{fileOrFolder.AbsolutePathOrFileName}'.");
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
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        {
            path += "python-3.11.8";
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system for Python integration.");
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
            // Try a relative path
            pythonExePath = Path.Combine(_currentDirectory, pythonExePath);
            if (!Directory.Exists(pythonExePath))
            {
                throw new DirectoryNotFoundException($"Python directory not found: {pythonExePath}");
            }
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

            throw new FileNotFoundException(
                $"Could not locate Python versioned DLL (pythonXXX.dll) in: {pythonExePath}");
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux uses libpython3.XX.so with version number
            var soFiles = Directory.GetFiles(Path.Combine(pythonExePath, "lib"), "libpython3.*.so")
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