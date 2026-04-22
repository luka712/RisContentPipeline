using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace RisContentPipeline.Tests;

public class Ktx2ConverterTests
{
    [Theory]
    [InlineData(".png")]
    [InlineData(".jpg")]
    [InlineData(".bmp")]
    public void ConvertFileToKtx2_ReadsDifferentImageFormats(string extension)
    {
        var inputPath = Path.Combine(Path.GetTempPath(), $"source-{Guid.NewGuid():N}{extension}");
        var outputPath = Path.Combine(Path.GetTempPath(), $"output-{Guid.NewGuid():N}.ktx2");

        try
        {
            using (var image = new Image<Rgba32>(4, 4, Color.CornflowerBlue))
            {
                switch (extension)
                {
                    case ".png":
                        image.Save(inputPath, new PngEncoder());
                        break;
                    case ".jpg":
                        image.Save(inputPath, new JpegEncoder());
                        break;
                    case ".bmp":
                        image.Save(inputPath, new BmpEncoder());
                        break;
                }
            }

            Ktx2Converter.ConvertFileToKtx2(inputPath, outputPath);

            Assert.True(File.Exists(outputPath));
            Assert.True(new FileInfo(outputPath).Length > 12);

            var header = new byte[12];
            using var output = File.OpenRead(outputPath);
            output.ReadExactly(header, 0, header.Length);

            Assert.Equal(new byte[]
            {
                0xAB, 0x4B, 0x54, 0x58, 0x20, 0x32, 0x30, 0xBB,
                0x0D, 0x0A, 0x1A, 0x0A
            }, header);
        }
        finally
        {
            if (File.Exists(inputPath))
            {
                File.Delete(inputPath);
            }

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
