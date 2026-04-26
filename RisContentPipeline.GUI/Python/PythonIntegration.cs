using Python.Runtime;
using System.Diagnostics;

namespace RisContentPipeline.GUI.Python;

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

    /// <summary>
    /// Processes the specified files using the Python script. 
    /// The script is expected to define a function that takes a file path and returns a ProcessResult.
    /// </summary>
    /// <param name="scriptName">The Python module name (without .py extension and without path separators).</param>
    /// <param name="filesToProcess">The files to process.</param>
    /// <returns>A collection of process results for each file.</returns>
    public void ProcessFiles(string scriptName, string[] filesToProcess)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("Python runtime not initialized. Call Initialize() first.");
        }

        if (_disposed)
            throw new ObjectDisposedException(nameof(PythonIntegration));

        Directory.CreateDirectory(_context.BuildDirectory);

        try
        {
            using (Py.GIL())
            {
                ImportModules();
                scriptName = scriptName.Split('/').Last().Split('\\').Last().Replace(".py", "");
                dynamic script = Py.Import(scriptName);
                dynamic builtins = Py.Import("builtins");

                // Call Python function for each file
                foreach (var file in filesToProcess)
                {
                    try
                    {
                        dynamic pyResult = script.process_file(file);

                        // We can ignore this, as there are no files to process.
                        // Usually this would indicate that script called create_ignore_result() to indicate that the file should be ignored.
                        if (pyResult["success"] && (pyResult["files"] is null || builtins.len(pyResult["files"]) == 0))
                        {
                            continue;
                        }

                        if (!pyResult["success"])
                        {
                            _context.BuildLogger.Success($"Error processing {file}: {pyResult["error"]}");
                        }

                        var files = pyResult["files"];

                        if (files["json"] != null)
                        {
                            var jsonContent = files["json"].ToString();
                            // Now save to build folder with same name but .json extension
                            var outputPath = Path.Combine(_context.BuildDirectory, $"{Path.GetFileNameWithoutExtension(file)}.json");
                            File.WriteAllText(outputPath, jsonContent);

                            _context.BuildLogger.Success($"Processed '{file}' successfully. Output saved to '{outputPath}'.");
                        }

                    }
                    catch (PythonException ex)
                    {
                        // TODO: to logger
                        _context.BuildLogger.Success($"Error processing {file}: {ex.Message}");
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
        string command = OperatingSystem.IsWindows() ? "where" : "which";

        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = "python",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start process to find Python.");
        }

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
        {
            throw new FileNotFoundException("Python executable not found. Please ensure Python is installed and in PATH.");
        }

        // First line is usually the main python.exe
        return output.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
    }

    /// <summary>
    /// Gets the Python DLL path from the Python executable path.
    /// </summary>
    private string GetPythonDllPath(string pythonExePath)
    {
        string directory = Path.GetDirectoryName(pythonExePath) ?? "";

        if (OperatingSystem.IsWindows())
        {
            // Try to find python3XX.dll in the Python directory (exclude python3.dll stub)
            var dllFiles = Directory.GetFiles(directory, "python3*.dll")
                .Where(f => !f.EndsWith("python3.dll", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(f => f) // Prefer higher versions
                .ToArray();

            if (dllFiles.Length > 0)
            {
                return dllFiles[0];
            }

            // Fallback: try common versions explicitly
            string[] commonVersions = { "313", "312", "311", "310", "39", "38" };
            foreach (var version in commonVersions)
            {
                string dllPath = Path.Combine(directory, $"python{version}.dll");
                if (File.Exists(dllPath))
                {
                    return dllPath;
                }
            }

            throw new FileNotFoundException($"Could not locate Python versioned DLL (pythonXXX.dll) in: {directory}");
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux uses libpython3.XX.so with version number
            var soFiles = Directory.GetFiles(Path.Combine(directory, "..", "lib"), "libpython3.*.so")
                .OrderByDescending(f => f)
                .ToArray();

            if (soFiles.Length > 0)
                return soFiles[0];

            throw new FileNotFoundException("Could not locate libpython3.XX.so");
        }
        else if (OperatingSystem.IsMacOS())
        {
            // macOS uses libpython3.XX.dylib with version number
            var dylibFiles = Directory.GetFiles(Path.Combine(directory, "..", "lib"), "libpython3.*.dylib")
                .OrderByDescending(f => f)
                .ToArray();

            if (dylibFiles.Length > 0)
                return dylibFiles[0];

            throw new FileNotFoundException("Could not locate libpython3.XX.dylib");
        }

        throw new PlatformNotSupportedException("Unsupported operating system for Python integration.");
    }
}