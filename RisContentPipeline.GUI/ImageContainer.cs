
using Eto.Drawing;

namespace RisContentPipeline
{
    /// <summary>
    /// The image container.
    /// </summary>
    internal class ImageContainer
    {
        /// <summary>
        /// The file path of the image. 
        /// This is used to identify the image and can be used for saving or referencing the image in the future.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The image.
        /// </summary>
        public Image Image { get; set; }
    }
}
