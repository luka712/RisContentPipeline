using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.Scripting.Python.Data
{
    public class PythonAssetFile
    {
        private readonly AssetFileOrFolder _fileOrFolder;
        private string? _content;

        /// <summary>
        /// The constructor takes a FileOrFolder object which represents a file or folder in the content pipeline's build directory.
        /// </summary>
        /// <param name="fileOrFolder"></param>
        public PythonAssetFile(AssetFileOrFolder fileOrFolder)
        {
            _fileOrFolder = fileOrFolder;
        }

        internal bool IsDirty { get; private set; }

        public string path => _fileOrFolder.PathOrFileName;

        public string absolute_path => _fileOrFolder.AbsolutePathOrFileName;

        public bool is_json => _fileOrFolder.IsJson;

        /// <summary>
        /// Gets or sets the content of the file if it is a JSON file.
        /// </summary>
        public string? content
        {
            get
            {
                if(!string.IsNullOrEmpty(_content))
                {
                    return _content;
                }

                if (!is_json)
                {
                    return null;
                }

                try
                {
                    _content =_fileOrFolder.GetContent();
                    return _content;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read JSON file: {ex.Message}");
                    return null;
                }
            }

            set
            {
                if(String.IsNullOrEmpty(value))
                {
                    return;
                }

                IsDirty = _content != value;
                _content = value;
                _fileOrFolder.SetAndWriteContent(_content);
            }
        }
    }
}
