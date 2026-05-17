namespace RisContentPipeline;

/// <summary>
/// Describes a single conversion request: what to convert, from which type,
/// to which type, and any pipeline-specific options.
/// </summary>
public class PipelineConversionItem
{
    /// <summary>
    /// The source type.
    /// </summary>
    public required string SourceType { get; set; }

    /// <summary>
    /// The target type.
    /// </summary>
    public required string TargetType { get; set; }

    /// <summary>
    /// The source item.
    /// </summary>
    public required object Source { get; set; }

    /// <summary>
    /// The options for the pipeline.
    /// </summary>
    public object? Options { get; set; }
}
