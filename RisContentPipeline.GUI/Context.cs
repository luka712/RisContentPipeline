using Eto.Drawing;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Extensions;
using RisContentPipeline.GUI.Python;
using RisContentPipeline.GUI.Services;
using RisTextureToolkit.Ktx;


namespace RisContentPipeline.GUI
{
    internal class Context
    {
        private Ktx2Converter _ktx2Pipeline = new Ktx2Converter();
        private List<FileOrFolder> _filesOrFolders = new List<FileOrFolder>();

        /// <summary>
        /// This event is triggered when the build process starts.
        /// </summary>
        public event Action? OnBuildStarted;

        public IReadOnlyList<FileOrFolder> FilesOrFolders => _filesOrFolders;

        /// <summary>
        /// The directory where the processed files will be saved. This is typically the "Build" folder in the content pipeline.
        /// </summary>
        public string BuildDirectory { get; set; } = "Build";

        /// <summary>
        /// The messanger instance for sending messages during the asset processing workflow.
        /// </summary>
        public BuildLogger BuildLogger { get; } = new BuildLogger();

        /// <summary>
        /// The list of Python scripts that can be used for custom processing steps in the content pipeline.
        /// </summary>
        public List<string> PythonScripts { get; } = ["InternalScripts/base_processor.py", "InternalScripts/texture_packer_png_to_ktx2.py"];

        /// <summary>
        /// Adds a file to the list of files or folders. 
        /// It checks the file extension and calls the appropriate method to handle the file based on its type (e.g., PNG, KTX2).
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal void AddFile(string filePath)
        {
            if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                AddPngFile(filePath);
            }
            else if(
                filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".xml", StringComparison.Ordinal))
            {
                AddGenericFile(filePath);
            }
        }

        /// <summary>
        /// Adds a PNG file to the list of files or folders.
        /// It loads the image from the specified file path and creates a FileOrFolder object with the loaded image, which is then added to the list of files or folders.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="parent">The parent, if any.</param>
        internal void AddPngFile(string filePath, FileOrFolder? parent = null)
        {
            var image = new Bitmap(filePath);
            var normalizedImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgba);
            using (var graphics = new Graphics(normalizedImage))
            {
                graphics.DrawImage(image, 0, 0);
            }
            FileOrFolder fileOrFolder = new FileOrFolder
            {
                AbsolutPathOrFileName = filePath,
                Image = normalizedImage
            };
            _filesOrFolders.Add(fileOrFolder);
        }

        internal void AddGenericFile(string filePath, FileOrFolder? parent = null)
        {
            _filesOrFolders.Add(new FileOrFolder()
            {
                AbsolutPathOrFileName = filePath,
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
            OnBuildStarted?.Invoke();
            BuildLogger.Clear();

            foreach (var fileOrFolder in _filesOrFolders)
            {
                if (fileOrFolder.Image != null)
                {
                    HandleImage(fileOrFolder);
                }
            }

            var files = _filesOrFolders
                .Where(x => !String.IsNullOrEmpty(x.AbsolutPathOrFileName))
                .Select(f => f.AbsolutPathOrFileName)
                .ToArray();
           
            if(PythonScripts.Any())
            {
                using var pythonIntegration = new PythonIntegration(this);
                pythonIntegration.Initialize();
                pythonIntegration.ProcessFiles(PythonScripts[1], files);
            }
        }

        private void HandleImage(FileOrFolder file)
        {
            var image = file.Image!;
            var bytes = image.GetBytes();
            var filePath = Path.Combine(BuildDirectory, Path.GetFileNameWithoutExtension(file.PathOrFileName));
            _ktx2Pipeline.Convert(
                bytes, 
                (uint)image.Width, (uint)image.Height, 
                $"{filePath}.ktx2", 
                new KtxBasisParams
                {
                    UseUastc = true,
                });
            BuildLogger.Success($"'{file.PathOrFileName}' has been converted to KTX2 format. Output file: '{filePath}.ktx2'");
        }
    }
}
