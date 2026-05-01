using RisKtx2;

namespace RisContentPipeline.Container
{
    /// <summary>
    /// The ImageContainer class is a simple data structure that holds information about an image, including its dimensions and raw pixel data.
    /// </summary>
    public class ImageContainer
    {
        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// The raw image data in bytes, typically in RGBA32 format (4 bytes per pixel).
        /// </summary>
        public byte[] Data { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// The data pointer. This is used when the image data is stored in unmanaged memory.
        /// </summary>
        public IntPtr DataPtr { get; set; } = IntPtr.Zero;

        /// <summary>
        /// The number of channels in the image (e.g., 4 for RGBA).
        /// </summary>
        public uint Channels { get; set; }

        /// <summary>
        /// The format of the image data.
        /// </summary>
        public VkFormat Format { get; set; } = VkFormat.R8G8B8A8_UNORM;
    }
}
