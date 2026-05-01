using Eto.Forms;
using RisContentPipeline.Container;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Scripting.Python;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Settings;
using RisKtx2;
using System.Text.Json;


namespace RisContentPipeline.GUI
{
    internal class Context
    {
        /// <summary>
        /// The collection of pipelines available in the content pipeline. 
        /// This includes various processing pipelines for handling different types of assets (e.g., images, audio, etc.).
        /// </summary>
        private readonly ContentPipelines _pipelines = new ();


        private readonly StbImageLoader _stbImageLoader = new StbImageLoader();
        private readonly List<Script> _buildScripts = [];

        private List<AssetFileOrFolder> _filesOrFolders = new List<AssetFileOrFolder>();

        /// <summary>
        /// The list of internal Python scripts that are included with the content pipeline.
        /// These scripts can be used for various processing tasks, such as converting PNG files to KTX2 format.
        /// </summary>
        internal IReadOnlyList<Script> InternalScripts = [
            new Script("InternalScripts/texture_packer_json_png_to_ktx2.py")
        ];

        /// <summary>
        /// This event is triggered when the build process starts.
        /// </summary>
        public event Action? OnBuildStarted;

        /// <summary>
        /// Fires when a new build script is added to the context.
        /// The event handler receives the path of the added script as a parameter.
        /// </summary>
        public EventHandler<Script>? OnBuildScriptAdded;

        public IReadOnlyList<AssetFileOrFolder> FilesOrFolders => _filesOrFolders;

        /// <summary>
        /// The directory where the processed files will be saved. This is typically the "Build" folder in the content pipeline.
        /// </summary>
        public string BuildDirectory { get; set; } = "./Build";

        /// <summary>
        /// The messanger instance for sending messages during the asset processing workflow.
        /// </summary>
        public BuildLogger BuildLogger { get; } = new BuildLogger();

        /// <summary>
        /// The settings related to KTX2 texture conversion.
        /// </summary>
        public Ktx2Settings Ktx2Settings { get; } = new Ktx2Settings();

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
                return AddPngFileAsync(filePath);
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
        internal Task AddPngFileAsync(string filePath, AssetFileOrFolder? parent = null)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                var data = _stbImageLoader.Load(filePath, out int width, out int height, out int channels);
                if (data == null)
                {
                    BuildLogger.ErrorAsync($"Failed to load image: '{filePath}'");
                    tcs.SetResult(false);
                    return;
                }

                AssetFileOrFolder fileOrFolder = new AssetFileOrFolder
                {
                    AbsolutePathOrFileName = filePath,
                    Image = new ImageContainer
                    {
                        Data = data,
                        Width = (uint)width,
                        Height = (uint)height,
                        Channels = (uint)channels,
                    }
                };
                _filesOrFolders.Add(fileOrFolder);
                tcs.SetResult(true);
            });

            return tcs.Task;

        }

        internal void AddGenericFile(string filePath, AssetFileOrFolder? parent = null)
        {
            _filesOrFolders.Add(new AssetFileOrFolder()
            {
                AbsolutePathOrFileName = filePath,
            });
        }

        /// <summary>
        /// Builds the content by processing each file or folder in the list. 
        /// It checks if the file has an associated image and calls the appropriate method to handle the image (e.g., converting it to KTX2 format).
        /// After processing all files, it checks if there are any Python scripts to execute and runs them using the PythonIntegration class, 
        /// passing the list of files to be processed by the scripts.
        /// </summary>
        internal void Build()
        {
            using var pythonIntegration = new PythonIntegration(this);
            if (BuildScripts.Any())
            {
                pythonIntegration.Initialize();
            }

            OnBuildStarted?.Invoke();
            BuildLogger.Clear();

            List<Task> tasks = new();

            foreach (var fileOrFolder in _filesOrFolders)
            {
                foreach (var script in BuildScripts)
                {
                    pythonIntegration.ProcessAsset(script, fileOrFolder);
                }


                if (fileOrFolder.Image != null)
                {
                    tasks.Add(HandleImageAsync(fileOrFolder));
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
                BuildLogger.InfoAsync($"Processing image: '{file.PathOrFileName}'");

                var source = file.Image;
                if (source == null)
                {
                    BuildLogger.ErrorAsync($"No image data found for file: '{file.PathOrFileName}'");
                    return;
                }

                var filePath = Path.Combine(BuildDirectory, Path.GetFileNameWithoutExtension(file.PathOrFileName));
                Ktx2Texture texture = null!;
                try
                {
                    KtxBasisParams? param = null;
                    if (Ktx2Settings.EncodeTarget != Ktx2EncodingTarget.NoEncoding)
                    {
                        param = new KtxBasisParams
                        {
                            UseUastc = Ktx2Settings.UseUastc,
                            // CompressionLevel = Ktx2Settings.CompressionLevel,
                            // QualityLevel = Ktx2Settings.QualityLevel
                        };
                    }

                    // Convert the image to KTX2 format using the content pipeline's texture processing pipeline
                    // and save the output to the specified build directory with a .ktx2 extension.
                    texture = _pipelines.CovertToKtx2(source, param, $"{filePath}.ktx2");
                }
                catch (Exception ex)
                {
                    BuildLogger.ErrorAsync($"Error processing image '{file.PathOrFileName}': {ex.Message}");
                    return;
                }
                BuildLogger.SuccessAsync($"'{file.PathOrFileName}' has been converted to KTX2 format. Output file: '{filePath}.ktx2'");
            });
        }

        public void LoadSession()
        {
            if (!File.Exists("session.json"))
                return;

            string jsonContent = File.ReadAllText("session.json");
            var session = JsonSerializer.Deserialize<Session>(jsonContent);
            if (session != null)
            {
                foreach (var scriptPath in session.BuildScripts.Distinct())
                {
                    AddBuildScript(new Script(scriptPath));
                }
            }
        }   

        public void SaveSession()
        {
            var session = new Session()
            {
                BuildScripts = BuildScripts.Select(s => s.FilePath).Distinct().ToList(),
            };

            string jsonContent = JsonSerializer.Serialize(session);
            File.WriteAllText("session.json", jsonContent);
        }
    }
}
