using RisContentPipeline.Ktx2;

namespace RisContentPipeline.Tests;

/// <summary>
/// Tests the pipeline system.
/// </summary>
public class PipelineSystemTest
{
    /// <summary>
    /// Test the conversion of a PNG image to KTX2.
    /// </summary>
    [Fact]
    public void Test_ConvertPngToKtx2()
    {
        var pipelineSystem = new PipelineSystem();
        var result = pipelineSystem.Convert("png", "ktx2",  new Ktx2PipelineSource()
        {
            FilePath = "Data/test.png"
        }, new Ktx2PipelineOptions()
        {
            UniversalBasisCompression = false,
            OutputPath = "Data/output_system_test.ktx2"
        });

        Assert.NotNull(result);
        Assert.True(File.Exists("Data/output_system_test.ktx2"));
    }

    /// <summary>
    /// Store multiple conversions and execute them all.
    /// </summary>
    [Fact]
    public void Test_StoreAndExecute()
    {
        var pipelineSystem = new PipelineSystem();
        pipelineSystem.StoreSourceAsset("png", "ktx2",  new Ktx2PipelineSource()
        {
            FilePath = "Data/test.png"
        }, new Ktx2PipelineOptions()
        {
            UniversalBasisCompression = false,
            OutputPath = "Data/output_system_test_a.ktx2"
        });
        
        pipelineSystem.StoreSourceAsset("png", "ktx2",  new Ktx2PipelineSource()
        {
            FilePath = "Data/test.png"
        }, new Ktx2PipelineOptions()
        {
            UniversalBasisCompression = false,
            OutputPath = "Data/output_system_test_b.ktx2"
        });

        var results = pipelineSystem.ConvertAll();
        
        Assert.True(results.All(x => x.Success));
        Assert.True(File.Exists("Data/output_system_test_a.ktx2"));
        Assert.True(File.Exists("Data/output_system_test_b.ktx2"));
    }
}