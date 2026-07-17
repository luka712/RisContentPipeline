using RisKtx2;

namespace RisContentPipeline.Ktx2;

/// <summary>
/// Provides functionality to convert images to KTX2 format with optional Basis Universal compression.
/// </summary>
public class Ktx2Pipeline : APipeline
{
    private const string NAME = "KTX2";
    private readonly StbImageLoader _stbImageLoader = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Ktx2Pipeline"/> class.
    /// </summary>
    public Ktx2Pipeline()
        : base(NAME, ["png"], ["ktx2"])
    {
    }

    /// <summary>
    /// Aligns <paramref name="value"/> up to the nearest multiple of <paramref name="alignment"/>.
    /// If <paramref name="alignment"/> is zero the original value is returned unchanged.
    /// </summary>
    private static int AlignUp(int value, int alignment)
    {
        if (alignment <= 0)
            return value;
        return (value + (alignment - 1)) & ~(alignment - 1);
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
        
        int alignment = 0;
        if (pipelineOptions.Encoding == Ktx2EncodingTarget.BASIS_UASTC 
            || pipelineOptions.Encoding == Ktx2EncodingTarget.BASIS_ETC1S
            || pipelineOptions.Encoding == Ktx2EncodingTarget.ASTC_4X4
            )
        {
            alignment = 4;
        }

        _stbImageLoader.VerticalFlip = pipelineOptions.FlipY;
        var image = _stbImageLoader.Load(sourceFilePath, 4, VkFormat.R8G8B8A8_UNORM);
        //
        // if (image.Channels == 3)
        // {
        //     image = _stbImageLoader.Align(image, alignment);
        // }
        // else if (image.Channels != 4)
        // {
        //     throw new InvalidOperationException("Unsupported number of channels.");
        // }
        
        var data = image.Bytes;
        var width = AlignUp(image.Width, alignment);
        var height = AlignUp(image.Height, alignment);
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
            BaseHeight = (uint) height,
            BaseWidth = (uint) width,
            VkFormat = VkFormat.R8G8B8A8_UNORM,
            NumLevels = (uint) mipLevels,
        }, KtxTextureCreateStorage.ALLOC_STORAGE);

        texture.SetImageFromMemory(0, 0, 0, data, (uint)data.Length);

        // If we have mip levels.
        if (genMipmaps)
        {
            var sourceWidth = width;
            var sourceHeight = height;
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
        bool uastc = pipelineOptions.Encoding == Ktx2EncodingTarget.BASIS_UASTC;
        bool etc1s = pipelineOptions.Encoding == Ktx2EncodingTarget.BASIS_ETC1S;
        if (uastc || etc1s)
        {
            KtxBasisParams basisParams = new ()
            {
                Uastc = uastc,
                ThreadCount = 0,
            };

            // If we do not have alpha channel, set its value to 1.
            if (channels == 3)
            {
                basisParams.InputSwizzle = ['r', 'g', 'b', '1'];
            }

            if (uastc)
            {
                basisParams.UastcFlags = pipelineOptions.UastcFlags;
            }
            else
            {
                basisParams.QualityLevel = pipelineOptions.QualityLevel;
            }

            texture.CompressBasis(basisParams);
        }
        else if (pipelineOptions.Encoding == Ktx2EncodingTarget.ASTC_4X4)
        {
            texture.CompressAstc(new KtxAstcParams()
            {
                QualityLevel = pipelineOptions.AstcQuality
            });
        }

        // Write the KTX2 texture to a file
        if (!string.IsNullOrEmpty(pipelineOptions.OutputPath))
        {
            texture.WriteToNamedFile(pipelineOptions.OutputPath);
        }

        return new Ktx2PipelineResult(texture);
    }
}
