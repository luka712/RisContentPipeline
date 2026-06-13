using RisContentPipeline.Ktx2;

namespace RisContentPipeline.Tests;

/// <summary>
/// Tests the KTX2 pipeline.
/// </summary>
public class Ktx2PipelineTest
{
   /// <summary>
   /// Test the conversion of a PNG image to KTX2.
   /// </summary>
   [Fact]
   public void Test_ConvertPngToKtx2()
   {
      var pipeline = new Ktx2Pipeline();
      var result = pipeline.Convert(new Ktx2PipelineSource()
      {
         FilePath = "Data/test.png"
      }, new Ktx2PipelineOptions()
      {
         GenerateMipmaps = true,
         Encoding = Ktx2EncodingTarget.BASIS_UASTC,
         OutputPath = "Data/output_pipeline_test.ktx2"
      });

      Assert.NotNull(result);
      Assert.True(File.Exists("Data/output_pipeline_test.ktx2"));
   }
}
