namespace RisContentPipeline.Ktx2;

/// <summary>
/// The options for the KTX2 pipeline.
/// </summary>
public class Ktx2PipelineOptions
{
    /// <summary>
    /// The output path for the KTX2 texture.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;
    
    /// <summary>
    /// Specifies whether to use Basis Universal compression for the KTX2 texture.
    /// </summary>
    public bool UniversalBasisCompression { get; set; } = false;
    
    /// <summary>
    /// Specifies whether to use UASTC compression for Basis Universal compression.
    /// Otherwise, ETC1S compression will be used.
    /// </summary>
    public bool UseUastc { get; set; } = false;
    
    /// <summary>
    /// Specifies whether to generate mipmaps for the KTX2 texture.
    /// </summary>
    public bool GenerateMipmaps { get; set; } = false;
    
}