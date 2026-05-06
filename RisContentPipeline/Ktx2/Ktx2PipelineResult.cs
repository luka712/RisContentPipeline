using RisKtx2;

namespace RisContentPipeline.Ktx2;

/// <summary>
/// The result of the KTX2 pipeline execution.
/// </summary>
public class Ktx2PipelineResult : PipelineResult
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="texture">The <see cref="Ktx2Texture"/>.</param>
    public Ktx2PipelineResult(Ktx2Texture texture)
    {
        Success = true;
        Result = texture;
    }
}