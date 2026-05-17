namespace RisContentPipeline.Generic;

/// <summary>
/// Represents a source file or folder in the generic pipeline.
/// </summary>
public record GenericPipelineSource
{
    /// <summary>
    /// The file path of the source file or folder.
    /// </summary>
    public required string FilePath { get; init; }
}
