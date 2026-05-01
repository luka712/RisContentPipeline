namespace RisContentPipeline.GUI.Data
{
    /// <summary>
    /// The Script class represents a script that can be used in the content pipeline for processing assets.
    /// </summary>
    internal class Script
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal Script(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// The file path of the script. 
        /// This can be either an internal script provided by the content pipeline or a custom script added by the user.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The name of the script, which is typically derived from the file path. This is used for display purposes in the GUI.
        /// </summary>
        public string Name => Path.GetFileName(FilePath);

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
