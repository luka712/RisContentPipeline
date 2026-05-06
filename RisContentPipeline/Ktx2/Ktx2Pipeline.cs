using RisKtx2;

namespace RisContentPipeline.Ktx2;

/// <summary>
/// Provides functionality to convert images to KTX2 format with optional Basis Universal compression.
/// </summary>
public class Ktx2Pipeline : APipeline
{
    private readonly StbImageLoader _stbImageLoader = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Ktx2Pipeline"/> class.
    /// </summary>
    public Ktx2Pipeline()
        : base(["png"], ["ktx2"])
    {
    }

    /// <inheritdoc/>
    public override PipelineResult Convert(object source, object? options)
    {
        var sourceFilePath = (source as Ktx2PipelineSource)?.FilePath ?? source as string;

        if (sourceFilePath is null)
        {
            throw new ArgumentException($"Cannot convert value from '{nameof(source)}'.");
        }
        
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException($"File not found: {sourceFilePath}");
        }
        
        Ktx2PipelineOptions pipelineOptions = options as Ktx2PipelineOptions ?? new Ktx2PipelineOptions();
        
        var image = _stbImageLoader.Load(sourceFilePath, 4);
        var data = image.Bytes;
        var width = (uint)image.Width;
        var height = (uint)image.Height;
        var channels = image.Channels;
        var genMipmaps = pipelineOptions.GenerateMipmaps;
        
        // Calculate mip levels
        var mipLevels = 1;
        if (genMipmaps)
        {
            var calculatedWidth = width;
            var calculatedHeight = height;
            while (calculatedWidth > 1 && calculatedHeight > 1)
            {
                mipLevels++;
                calculatedWidth /= 2;
                calculatedHeight /= 2;
            }
        }

        // Create KTX2 texture with RGBA8 format
        Ktx2Texture texture = new Ktx2Texture(new KtxTextureCreateInfo
        {
            BaseHeight = height,
            BaseWidth = width,
            VkFormat = VkFormat.R8G8B8A8_UNORM,
            NumLevels = (uint) mipLevels
        });

        texture.SetImageFromMemory(0, 0, 0, data, (uint)data.Length);

        // If we have mip levels.
        if (genMipmaps)
        {
            var sourceWidth = (int)width;
            var sourceHeight = (int)height;
            var targetWidth = sourceWidth / 2;
            var targetHeight = sourceHeight / 2;
            var mipData = data;
            uint i = 1;
            do
            {
                mipData = _stbImageLoader.Resize(
                    mipData, sourceWidth, sourceHeight,
                    channels,
                    targetWidth, targetHeight);

                texture.SetImageFromMemory(i, 0, 0, mipData, (uint)mipData.Length);

                sourceWidth = targetWidth;
                sourceHeight = targetHeight;
                targetWidth = sourceWidth / 2;
                targetHeight = sourceHeight / 2;
                i++;
            } while (i < mipLevels);
        }

        // Apply Basis Universal compression if parameters are provided
        if (pipelineOptions.UniversalBasisCompression)
        {
            texture.CompressBasis(new KtxBasisParams()
            {
                Uastc = pipelineOptions.UseUastc,
            });
        }

        // Write the KTX2 texture to a file
        if (!String.IsNullOrEmpty(pipelineOptions.OutputPath))
        {
            texture.WriteToNamedFile(pipelineOptions.OutputPath);
        }

        return new Ktx2PipelineResult(texture);
    }
}