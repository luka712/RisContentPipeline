using Eto.Drawing;


namespace RisContentPipeline.GUI
{
    /// <summary>
    /// The Icons class is responsible for loading and providing access to the icons used in the asset view of the application.
    /// </summary>
    internal class Icons
    {
        /// <summary>
        /// The folder icon used in the asset view to represent folders.
        /// </summary>
        public static Icon FolderIcon { get; private set; } = null!;

        /// <summary>
        /// The image icon used in the asset view to represent image files.
        /// </summary>
        public static Icon ImageIcon { get; private set; } = null!;

        /// <summary>
        /// The file icon used in the asset view to represent generic files that are not images or folders.
        /// </summary>
        public static Icon FileIcon { get; private set; } = null!;

        /// <summary>
        /// The check icon used in the asset view to indicate that an asset has been successfully processed or is valid.
        /// </summary>
        public static Icon CheckIcon { get; private set; } = null!;

        /// <summary>
        /// The Python script icon.
        /// </summary>
        public static Icon PythonIcon { get; private set; } = null!;

        /// <summary>
        /// The info icon used in the asset view to indicate informational messages or warnings related to assets.
        /// </summary>
        public static Icon InfoIcon { get; private set; } = null!;

        private static Icon LoadIcon(string path)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var bitmap = new Bitmap($"{directory}/{path}");
            return bitmap.WithSize(16, 16);
        }

        /// <summary>
        /// Loads the icons from the specified file paths.
        /// This method should be called during application initialization to ensure that the icons are available for use in the asset view.
        /// </summary>
        public static void Load()
        {
            FolderIcon = LoadIcon("Icons/folder-solid.png");
            ImageIcon = LoadIcon("Icons/image-solid.png");
            FileIcon = LoadIcon("Icons/file-solid.png");
            CheckIcon = LoadIcon("Icons/check-solid.png");
            PythonIcon = LoadIcon("Icons/python-brands-solid.png");
            InfoIcon = LoadIcon("Icons/circle-info-solid.png");
        }
    }
}
