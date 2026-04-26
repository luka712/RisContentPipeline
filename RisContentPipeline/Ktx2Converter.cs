using RisTextureToolkit.Ktx;

namespace RisContentPipeline;

/// <summary>
/// Provides functionality to convert images to KTX2 format with optional Basis Universal compression.
/// </summary>
public class Ktx2Converter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ktx2Converter"/> class.
    /// </summary>
    public Ktx2Converter()
    {
    }

    /// <summary>
    /// The output folder where the converted KTX2 textures will be saved.
    /// This is relative to the project root and can be customized as needed.
    /// </summary>
    public string BuildFolder { get; set; } = "ContentBuild";

    /// <summary>
    /// Converts raw RGBA32 pixel data to KTX2 format and writes it to the specified output path.
    /// </summary>
    /// <param name="bytes">The raw RGBA32 pixel data (4 bytes per pixel).</param>
    /// <param name="imageWidth">The width of the image in pixels.</param>
    /// <param name="imageHeight">The height of the image in pixels.</param>
    /// <param name="outputPath">The file path where the KTX2 texture will be saved. Must end with .ktx2 extension.</param>
    /// <param name="ktxBasisParams">Optional Basis Universal compression parameters. If provided, the texture will be compressed to universal basis standard.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bytes"/> or <paramref name="outputPath"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="outputPath"/> does not end with .ktx2 extension or when byte array size doesn't match image dimensions.</exception>
    public void Convert
        (byte[] bytes, 
        uint imageWidth, uint imageHeight, 
        string outputPath,
        KtxBasisParams? ktxBasisParams)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        if (!outputPath.EndsWith(".ktx2", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Output path must have a .ktx2 extension.", nameof(outputPath));
        }

        // By default, use UASTC compression if no parameters are provided
        ktxBasisParams ??= new KtxBasisParams { UseUastc = true };

        // Validate byte array size matches expected dimensions (4 bytes per RGBA32 pixel)
        uint expectedSize = imageWidth * imageHeight * 4;
        if (bytes.Length != expectedSize)
        {
            throw new ArgumentException(
                $"Byte array size ({bytes.Length}) doesn't match expected size ({expectedSize}) for {imageWidth}x{imageHeight} RGBA32 image.",
                nameof(bytes));
        }

        // Create KTX2 texture with RGBA8 format
        Ktx2Texture texture = new Ktx2Texture(new KtxTextureCreateInfo
        {
            BaseHeight = imageHeight,
            BaseWidth = imageWidth,
            VkFormat = VkFormat.R8G8B8A8_UNORM
        });


        // Set image data directly from the byte array
        texture.SetImageFromMemory(0, 0, 0, bytes, (ulong)bytes.Length);

        // Apply Basis Universal compression if parameters are provided
        if (ktxBasisParams.HasValue)
        {
            texture.CompressBasis(ktxBasisParams.Value);
        }

        // Write the texture to file
        texture.WriteToNamedFile(outputPath);
    }
}
