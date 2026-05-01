using Eto.Drawing;
using RisContentPipeline.Container;
using System.Text.Json;

namespace RisContentPipeline.GUI.Data
{
    /// <summary>
    /// File or folder of other folders or files, which contains underlying data file,
    /// typically an image, audio, or other asset that can be processed by the content pipeline.
    /// </summary>
    public class AssetFileOrFolder
    {
        private string? _content;
        private string _absolutPathOrFileName = string.Empty;

        /// <summary>
        /// The absolute file path of the file or folder. 
        /// This is used for loading the file and should not be modified by the user.
        /// </summary>
        public string AbsolutePathOrFileName
        {
            get => _absolutPathOrFileName;
            set
            {
                _absolutPathOrFileName = value;
                PathOrFileName = System.IO.Path.GetFileName(value);
            }
        }

        /// <summary>
        /// The file path of the the file or folder.
        /// </summary>
        public string PathOrFileName { get; private set; }

        /// <summary>
        /// The image file.
        /// </summary>
        public ImageContainer? Image { get; set; }

        /// <summary>
        /// Gets the content of the file if it is a JSON file.
        /// Returns null if the file is not a JSON file or if there was an error reading the file.
        /// </summary>
        /// <returns>The content.</returns>
        public string? GetContent()
        {
            if(!string.IsNullOrEmpty(_content))
            {
                return _content;
            }

            if (!IsJson)
            {
                return null;
            }

            try
            {
                _content = File.ReadAllText(AbsolutePathOrFileName);

                // Validate that the content is valid JSON
                JsonDocument.Parse(_content);

                return _content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read JSON file: {ex.Message}");
                return null;
            }
        }

        public void SetAndWriteContent(string content)
        {
            if (!IsJson)
            {
                throw new InvalidOperationException("Can only set content for JSON files.");
            }

            try
            {
                // Validate that the content is valid JSON
                JsonDocument.Parse(content);
                _content = content;
                File.WriteAllText(PathOrFileName, _content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write JSON file: {ex.Message}");
            }
        }

        /// <summary>
        /// Indicates if file is JSON file.
        /// </summary>
        public bool IsJson => PathOrFileName?.EndsWith(".json") == true;

        /// <summary>
        /// Indices if file is XML file.
        /// </summary>
        public bool IsXml => PathOrFileName?.EndsWith(".xml") == true;

        /// <summary>
        /// The child folders or files of this folder. 
        /// </summary>
        public List<AssetFileOrFolder> Children { get; set; } = new();
    }
}
