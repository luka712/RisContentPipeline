using RisContentPipeline.Container;
using RisKtx2;

namespace RisContentPipeline;

/// <summary>
/// Provides functionality to convert images to KTX2 format with optional Basis Universal compression.
/// </summary>
public class ImageToKtx2Pipeline : IContentPipeline<ImageContainer, Ktx2Texture>
{
    private readonly StbImageLoader _stbImageLoader = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageToKtx2Pipeline"/> class.
    /// </summary>
    public ImageToKtx2Pipeline()
    {
    }

    /// <inheritdoc/>
    public Ktx2Texture Convert(ImageContainer source, object? options = null)
    {
        var pointer = source.DataPtr;
        var data = source.Data;
        var width = source.Width;
        var height = source.Height;
        var channels = source.Channels;

        if (pointer == IntPtr.Zero && (data is null || data.Length == 0))
        {
            throw new ArgumentException("Image data is missing.");
        }

        KtxBasisParams? ktxBasisParams = options as KtxBasisParams?;
        uint? quality = options as uint?;

        // Create KTX2 texture with RGBA8 format
        Ktx2Texture texture = new Ktx2Texture(new KtxTextureCreateInfo
        {
            BaseHeight = height,
            BaseWidth = width,
            VkFormat = VkFormat.R8G8B8A8_UNORM
        });

        // Set image data directly from the byte array
        if (data?.Length > 0)
        {
            texture.SetImageFromMemory(0, 0, 0, data, (uint)data.Length);
        }
        else
        {
            texture.SetImageFromMemory(0, 0, 0, pointer,
                width * height * channels); // Assuming 4 bytes per pixel for RGBA8
        }

        // If we have mip levels.
        if (source.GenerateMipmaps)
        {
            var sourceWidth = (int)width;
            var sourceHeight = (int)height;
            var targetWidth = sourceWidth / 2;
            var targetHeight = sourceHeight / 2;
            var mipData = source.Data;
            uint i = 1;
            do
            {
                mipData = _stbImageLoader.Resize(
                    mipData, sourceWidth, sourceHeight,
                    (int)source.Channels,
                    targetWidth, targetHeight);

                texture.SetImageFromMemory(i, 0, 0, mipData, (uint)mipData.Length);

                sourceWidth = targetWidth;
                sourceHeight = targetHeight;
                targetWidth = sourceWidth / 2;
                targetHeight = sourceHeight / 2;
                i++;
            } while (targetWidth > 1 && targetHeight > 1);
        }

        // Apply Basis Universal compression if parameters are provided
        if (ktxBasisParams.HasValue)
        {
            texture.CompressBasis(ktxBasisParams.Value);
        }
        else if (quality.HasValue)
        {
            // texture.CompressBasis(quality.Value);
        }

        return texture;
    }
}