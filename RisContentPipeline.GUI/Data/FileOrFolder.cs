using Eto.Drawing;

namespace RisContentPipeline.GUI.Data
{
    /// <summary>
    /// File or folder of other folders or files, which contains underlying data file,
    /// typically an image, audio, or other asset that can be processed by the content pipeline.
    /// </summary>
    internal class FileOrFolder
    {
        private string _absolutPathOrFileName = string.Empty;

        /// <summary>
        /// The absolute file path of the file or folder. 
        /// This is used for loading the file and should not be modified by the user.
        /// </summary>
        public string AbsolutPathOrFileName
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
        public Bitmap? Image { get; set; }

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
        public List<FileOrFolder> Children { get; set; } = new();
    }
}
