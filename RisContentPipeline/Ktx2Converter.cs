using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RisContentPipeline;

public static class Ktx2Converter
{
    private static readonly byte[] Ktx2Identifier =
    [
        0xAB, 0x4B, 0x54, 0x58, 0x20, 0x32, 0x30, 0xBB,
        0x0D, 0x0A, 0x1A, 0x0A
    ];

    private const uint VkFormatR8G8B8A8Unorm = 37;

    public static void ConvertFileToKtx2(string inputFilePath, string outputFilePath)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Input file path is required.", nameof(inputFilePath));
        }

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            throw new ArgumentException("Output file path is required.", nameof(outputFilePath));
        }

        using var inputStream = File.OpenRead(inputFilePath);
        using var outputStream = File.Create(outputFilePath);
        ConvertToKtx2(inputStream, outputStream);
    }

    public static void ConvertToKtx2(Stream inputStream, Stream outputStream)
    {
        ArgumentNullException.ThrowIfNull(inputStream);
        ArgumentNullException.ThrowIfNull(outputStream);

        using var image = Image.Load<Rgba32>(inputStream);

        var pixelData = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixelData);

        WriteKtx2(outputStream, image.Width, image.Height, pixelData);
    }

    private static void WriteKtx2(Stream output, int width, int height, byte[] pixelData)
    {
        using var writer = new BinaryWriter(output, System.Text.Encoding.UTF8, leaveOpen: true);

        // KTX2 fixed header size: 12-byte identifier + 68-byte header block.
        const uint headerByteSize = 80;
        // One Level Index entry is 3 x uint64 (offset, length, uncompressed length).
        const uint levelIndexByteSize = 24;
        var imageByteOffset = headerByteSize + levelIndexByteSize;
        var imageByteLength = (ulong)pixelData.Length;

        writer.Write(Ktx2Identifier);
        writer.Write(VkFormatR8G8B8A8Unorm);
        writer.Write(1u);
        writer.Write((uint)width);
        writer.Write((uint)height);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(1u);
        writer.Write(1u);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(0ul);
        writer.Write(0ul);

        writer.Write((ulong)imageByteOffset);
        writer.Write(imageByteLength);
        writer.Write(imageByteLength);

        writer.Write(pixelData);
    }
}
