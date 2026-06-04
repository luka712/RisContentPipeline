using Eto.Forms;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Scripting.Python;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Settings;
using RisKtx2;
using System.Text.Json;
using RisContentPipeline.Ktx2;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Persistance;

namespace RisContentPipeline.GUI
{
    /// <summary>
    /// The application-wide context shared by views, modals, and the build pipeline.
    /// It owns the active <see cref="IPipelineSystem"/>, the list of imported assets, the
    /// list of build scripts and the global KTX2 settings, and is also responsible for
    /// loading/saving the user session and orchestrating the build process.
    /// </summary>
    internal class Context : IDisposable
    {
        // ----- Static configuration -----------------------------------------------------

        /// <summary>
        /// The directory (relative to the executable) that contains the built-in Python
        /// scripts shipped with the application.
        /// </summary>
        private const string INTERNAL_SCRIPTS_DIRECTORY = "InternalScripts";

        /// <summary>
        /// The directory (relative to the executable) that contains user-provided Python
        /// scripts. The folder is optional and is only loaded when it exists.
        /// </summary>
        private const string USER_SCRIPTS_DIRECTORY = "UserScripts";

        /// <summary>
        /// The path of the JSON file used to persist the session between application runs.
        /// </summary>
        private const string SESSION_FILE = "session.json";
        
        /// <summary>
        /// The path of the JSON file used to persist the preferences between application runs.
        /// </summary>
        private const string PREFERENCES_FILE = "preferences.json";

        // ----- Fields -------------------------------------------------------------------

        /// <summary>
        /// The collection of pipelines available in the content pipeline. 
        /// This includes various processing pipelines for handling different types of assets (e.g., images, audio, etc.).
        /// </summary>
        public IPipelineSystem PipelineSystem = new PipelineSystem();

        private readonly StbImageLoader _stbImageLoader = new StbImageLoader();
        private PythonIntegration? _pythonIntegration;
        private readonly List<Script> _buildScripts = [];
        private readonly List<Script> _internalScripts = [];

        private List<AssetFileOrFolder> _filesOrFolders = new List<AssetFileOrFolder>();

        /// <summary>
        /// Initializes a new <see cref="Context"/> and wires up build logging for the
        /// underlying <see cref="IPipelineSystem"/>.
        /// </summary>
        public Context()
        {
            LoadInternalScripts();

            PipelineSystem.OnConvertAllStarted += (sender, args) =>
            {
                MessageLogger.InfoAsync($"Build started. Total items: {args.TotalItems}");
            };

            PipelineSystem.OnConvertAllFinished += (sender, args) =>
            {
                MessageLogger.InfoAsync($"Build finished. Total items: {args.TotalItems}");
            };
        }

        /// <summary>
        /// The port used for local server communication.
        /// </summary>
        internal int LocalServerPort = 8787;

        /// <summary>
        /// The list of internal Python scripts that are included with the content pipeline.
        /// The list is populated at construction time by <see cref="LoadInternalScripts"/>
        /// from the <see cref="INTERNAL_SCRIPTS_DIRECTORY"/> directory.
        /// These scripts can be used for various processing tasks, such as converting PNG files to KTX2 format.
        /// </summary>
        internal IReadOnlyList<Script> InternalScripts => _internalScripts;

        /// <summary>
        /// This event is triggered when the build process starts.
        /// </summary>
        public event Action? OnBuildStarted;

        /// <summary>
        /// Fires when a new build script is added to the context.
        /// </summary>
        public event EventHandler<Script>? OnBuildScriptAdded;

        /// <summary>
        /// Fires when a build script is removed from the context.
        /// </summary>
        public event EventHandler<Script>? OnBuildScriptRemoved;

        public IReadOnlyList<AssetFileOrFolder> FilesOrFolders => _filesOrFolders;

        /// <summary>
        /// The directory where the processed files will be saved. This is typically the "Build" folder in the content pipeline.
        /// </summary>
        public string BuildDirectory { get; set; } = "./Build";

        /// <summary>
        /// The messanger instance for sending messages during the asset processing workflow.
        /// </summary>
        public MessageLogger MessageLogger { get; } = new MessageLogger();

        /// <summary>
        /// The preferences for the content pipeline app.
        /// </summary>
        public Preferences Preferences { get; private set; } = new ();

        /// <summary>
        /// The list of scripts to be executed during the build process.
        /// This can include both internal scripts provided by the content pipeline and custom scripts added by the user.
        /// </summary>
        public IReadOnlyList<Script> BuildScripts => _buildScripts;

        /// <summary>
        /// Invoked when an item (file or folder) is selected in the asset view. 
        /// The event handler receives the selected item as a parameter, allowing the application to respond to the selection (e.g., by displaying details about the selected item or enabling certain actions based on the selection).
        /// </summary>
        public Action<AssetFileOrFolder>? OnItemSelected { get; internal set; }

        /// <summary>
        /// Adds a build script to the list of scripts to be executed during the build process.
        /// </summary>
        /// <param name="script">The <see cref="Script"/>.</param>
        public void AddBuildScript(Script script)
        {
            _buildScripts.Add(script);
            OnBuildScriptAdded?.Invoke(this, script);
        }

        /// <summary>
        /// Removes a build script from the list of scripts to be executed during the build process.
        /// </summary>
        /// <param name="name">The script name.</param>
        internal void RemoveBuildScript(string name)
        {
            var script = _buildScripts.FirstOrDefault(s => s.Name == name);
            if (script != null)
            {
                RemoveBuildScript(script);
            }
        }

        /// <summary>
        /// Removes a build script from the list of scripts to be executed during the build process.
        /// </summary>
        /// <param name="script">The <see cref="Script"/>.</param>
        internal void RemoveBuildScript(Script script)
        {
            if (_buildScripts.Remove(script))
            {
                OnBuildScriptRemoved?.Invoke(this, script);
            }
        }

        /// <summary>
        /// Adds multiple build scripts to the list of scripts to be executed during the build process.
        /// </summary>
        /// <param name="scripts">The collection of <see cref="Script"/> items.</param>
        public void AddBuildScripts(IEnumerable<Script> scripts)
        {
            foreach (var script in scripts)
            {
                AddBuildScript(script);
            }
        }

        /// <summary>
        /// Adds a file to the list of files or folders. 
        /// It checks the file extension and calls the appropriate method to handle the file based on its type (e.g., PNG, KTX2).
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal Task AddFileAsync(string filePath)
        {
            if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                AddPngFile(filePath);
            }
            else if (
                filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".xml", StringComparison.Ordinal))
            {
                AddGenericFile(filePath);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a PNG file to the list of files or folders.
        /// It loads the image from the specified file path and creates a FileOrFolder object with the loaded image, which is then added to the list of files or folders.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="parent">The parent, if any.</param>
        internal void AddPngFile(string filePath, AssetFileOrFolder? parent = null)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            AssetFileOrFolder fileOrFolder = new AssetFileOrFolder
            {
                AbsolutePathOrFileName = filePath,
                Image = new ImageContainer()
                {
                    FilePath = filePath,
                    Ktx2ExportSettings = Preferences.Ktx2GlobalSettings.Copy()
                }
            };
            _filesOrFolders.Add(fileOrFolder);
            tcs.SetResult(true);
        }

        internal void AddGenericFile(string filePath, AssetFileOrFolder? parent = null)
        {
            _filesOrFolders.Add(new AssetFileOrFolder()
            {
                AbsolutePathOrFileName = filePath,
            });
        }

        /// <summary>
        /// Builds the content by queuing every imported file or folder into the
        /// <see cref="PipelineSystem"/> and invoking <see cref="IPipelineSystem.ConvertAll"/>.
        /// Before queuing, all registered build scripts get their <c>before_build</c>
        /// callback invoked, and any previously stored assets are cleared from the
        /// pipeline system to avoid duplicate processing on successive builds.
        /// After conversion, each script's <c>after_build</c> callback is invoked.
        /// </summary>
        internal void Build()
        {
            if (!Directory.Exists(BuildDirectory))
            {
                Directory.CreateDirectory(BuildDirectory);
            }

            if (BuildScripts.Any())
            {
                _pythonIntegration = _pythonIntegration ?? new PythonIntegration(this);
                _pythonIntegration.Initialize();
            }

            // Run before build scripts.
            foreach (var script in BuildScripts)
            {
                _pythonIntegration?.BeforeBuild(script);
            }

            OnBuildStarted?.Invoke();
            MessageLogger.Clear();

            // Make sure we don't accumulate stored assets from previous builds.
            PipelineSystem.ClearStoredAssets();

            // Queue all files into the pipeline system.
            foreach (var fileOrFolder in _filesOrFolders)
            {
                QueueFileForBuild(fileOrFolder);
            }

            _ = PipelineSystem.ConvertAllAsync();

            // Run after build scripts.
            foreach (var script in BuildScripts)
            {
                _pythonIntegration?.AfterBuild(script);
            }
        }

        /// <summary>
        /// Stores a single asset in the pipeline system based on its detected type.
        /// </summary>
        /// <param name="fileOrFolder">The asset to be queued for processing.</param>
        private void QueueFileForBuild(AssetFileOrFolder fileOrFolder)
        {
            var fileName = fileOrFolder.PathOrFileName;
            if (string.IsNullOrEmpty(fileName))
                return;

            if (fileOrFolder.Image != null)
            {
                var image = fileOrFolder.Image;
                var ktx2Settings = image.Ktx2ExportSettings;
                var filePath = Path.Combine(AppContext.BaseDirectory, BuildDirectory,
                    Path.GetFileNameWithoutExtension(fileName));
                var uastc = ktx2Settings.EncodeTarget == Ktx2EncodingTarget.BASIS_UASTC;

                PipelineSystem.StoreSourceAsset("png", "ktx2", new Ktx2PipelineSource()
                {
                    FilePath = fileOrFolder.AbsolutePathOrFileName,
                }, new Ktx2PipelineOptions()
                {
                    GenerateMipmaps = ktx2Settings.GenerateMipmaps,
                    OutputPath = $"{filePath}.ktx2",
                    UniversalBasisCompression = ktx2Settings.EncodeTarget == Ktx2EncodingTarget.BASIS_ETC1S
                                                || uastc,
                    UseUastc = uastc,
                    QualityLevel =ktx2Settings.GetQualityLevelValue(),
                });
            }
            else if (fileOrFolder.IsJson)
            {
                var fileType = fileName.Split('.').LastOrDefault();
                if (fileType != null)
                {
                    PipelineSystem.StoreSourceAsset(fileType, IPipeline.ANY_TYPE, new GenericPipelineSource()
                    {
                        FilePath = fileOrFolder.AbsolutePathOrFileName,
                    }, new GenericPipelineOptions()
                    {
                        OutputPath =
                            Path.Combine(BuildDirectory, Path.GetFileName(fileOrFolder.AbsolutePathOrFileName)),
                    });
                }
            }
        }

        internal void AsyncInvoke(Action action)
        {
            Application.Instance.AsyncInvoke(action);
        }

        private Task HandleImageAsync(AssetFileOrFolder file)
        {
            return Task.Run(() =>
            {
                MessageLogger.InfoAsync($"Processing image: '{file.PathOrFileName}'");

                var source = file.Image;
                if (source == null)
                {
                    MessageLogger.ErrorAsync($"No image data found for file: '{file.PathOrFileName}'");
                    return;
                }

                var filePath = Path.Combine(BuildDirectory,
                    Path.GetFileNameWithoutExtension(file.PathOrFileName ?? string.Empty));
                try
                {
                    // Convert the image to KTX2 format using the content pipeline's texture processing pipeline
                    // and save the output to the specified build directory with a .ktx2 extension.
                    var ktxPipelineSource = new Ktx2PipelineSource()
                    {
                        FilePath = source.FilePath ?? string.Empty,
                    };

                    var uastcEncoding = Preferences.Ktx2GlobalSettings.EncodeTarget == Ktx2EncodingTarget.BASIS_UASTC;

                    var ktxPipelineOptions = new Ktx2PipelineOptions()
                    {
                        GenerateMipmaps = source.Ktx2ExportSettings.GenerateMipmaps,
                        UniversalBasisCompression = Preferences.Ktx2GlobalSettings.EncodeTarget == Ktx2EncodingTarget.BASIS_ETC1S
                                                    || uastcEncoding,
                        UseUastc = uastcEncoding,
                        OutputPath = $"{filePath}.ktx2",
                    };

                    var result = PipelineSystem.Convert("png", "ktx2", ktxPipelineSource, ktxPipelineOptions);

                    if (!result.Success)
                    {
                        MessageLogger.ErrorAsync($"Failed to convert image '{file.PathOrFileName}' to KTX2 format.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageLogger.ErrorAsync($"Error processing image '{file.PathOrFileName}': {ex.Message}");
                    return;
                }

                MessageLogger.SuccessAsync(
                    $"'{file.PathOrFileName}' has been converted to KTX2 format. Output file: '{filePath}.ktx2'");
            });
        }

        /// <summary>
        /// Loads the user session from <see cref="SESSION_FILE"/>, restoring the list of
        /// previously configured build scripts. Silently does nothing if no session file
        /// exists.
        /// </summary>
        public void LoadSession()
        {
            if (!File.Exists(SESSION_FILE))
                return;

            try
            {
                string jsonContent = File.ReadAllText(SESSION_FILE);
                var session = JsonSerializer.Deserialize<Session>(jsonContent);
                if (session != null)
                {
                    foreach (var scriptPath in session.BuildScripts.Distinct())
                    {
                        if (!File.Exists(scriptPath))
                        {
                            MessageLogger.WarnAsync($"Build script '{scriptPath}' cannot be found.");
                            continue;
                        }

                        AddBuildScript(new Script(scriptPath));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageLogger.Error($"Failed to load session from '{SESSION_FILE}': {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the user preferences from <see cref="PREFERENCES_FILE"/>, restoring
        /// </summary>
        public async Task LoadPreferencesAsync()
        {
            if (!File.Exists(PREFERENCES_FILE))
            {
                return;
            }
            
            try
            {
                string jsonContent = await File.ReadAllTextAsync(PREFERENCES_FILE);
                var preferences = JsonSerializer.Deserialize<Preferences>(jsonContent);
                if (preferences != null)
                {
                    Preferences = preferences;
                }
            }
            catch (Exception ex)
            {
                MessageLogger.Error($"Failed to load session from '{SESSION_FILE}': {ex.Message}");
            }
        }

        public void SaveSession()
        {
            var session = new Session()
            {
                BuildScripts = BuildScripts.Select(s => s.FilePath).Distinct().ToList(),
            };

            string jsonContent = JsonSerializer.Serialize(session);
            File.WriteAllText(SESSION_FILE, jsonContent);
        }

        /// <summary>
        /// Saves the current preferences to <see cref="PREFERENCES_FILE"/>.
        /// </summary>
        public Task SavePreferencesAsync()
        {
            string jsonContent = JsonSerializer.Serialize(Preferences);
            return File.WriteAllTextAsync(PREFERENCES_FILE, jsonContent);
        }

        /// <summary>
        /// Cleans the build directory by recursively deleting all generated artifacts.
        /// Pipeline state and imported assets are preserved so a subsequent
        /// <see cref="Build"/> can recreate the output from scratch.
        /// </summary>
        internal void Clean()
        {
            try
            {
                if (Directory.Exists(BuildDirectory))
                {
                    Directory.Delete(BuildDirectory, recursive: true);
                    MessageLogger.Info($"Cleaned build directory: '{BuildDirectory}'");
                }
                else
                {
                    MessageLogger.Info($"Nothing to clean. Build directory does not exist: '{BuildDirectory}'");
                }
            }
            catch (Exception ex)
            {
                MessageLogger.Error($"Failed to clean build directory '{BuildDirectory}': {ex.Message}");
            }
        }

        /// <summary>
        /// Performs a clean and full re-build of all imported assets.
        /// </summary>
        internal void Rebuild()
        {
            Clean();
            Build();
        }

        /// <summary>
        /// Discovers all <c>.py</c> scripts inside the <see cref="INTERNAL_SCRIPTS_DIRECTORY"/>
        /// folder (relative to the working directory and to the executable directory) and
        /// caches them in <see cref="_internalScripts"/>.
        /// Pure helper module files prefixed with <c>base_</c> or <c>__</c> (such as
        /// <c>base_processor.py</c> and <c>__template__.py</c>) are skipped because they
        /// are intended to be imported by other scripts rather than executed directly.
        /// </summary>
        private void LoadInternalScripts()
        {
            _internalScripts.Clear();
            var directory = ResolveScriptDirectory(INTERNAL_SCRIPTS_DIRECTORY);
            if (directory == null)
            {
                return;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.py", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(file);
                if (name.StartsWith("base_", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("__", StringComparison.Ordinal))
                {
                    continue;
                }

                _internalScripts.Add(new Script(file));
            }
        }

        /// <summary>
        /// Resolves a script directory relative to either the current working directory
        /// or the executable directory. Returns <c>null</c> if neither location exists.
        /// </summary>
        private static string? ResolveScriptDirectory(string relativeDirectory)
        {
            var fromCwd = Path.GetFullPath(relativeDirectory);
            if (Directory.Exists(fromCwd))
            {
                return fromCwd;
            }

            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (assemblyDir != null)
            {
                var fromAssembly = Path.Combine(assemblyDir, relativeDirectory);
                if (Directory.Exists(fromAssembly))
                {
                    return fromAssembly;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _pythonIntegration?.Dispose();
        }
    }
}