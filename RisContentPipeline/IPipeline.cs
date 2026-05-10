namespace RisContentPipeline;

/// <summary>
/// The pipeline within the content pipeline.
/// </summary>
public interface IPipeline
{
    public const string ANY_TYPE = "*";

    /// <summary>
    /// The name of the pipeline.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The name of the pipeline.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The source types that this pipeline can process.
    /// </summary>
    IReadOnlyList<string> SourceTypes { get; }

    /// <summary>
    /// The target types that this pipeline can produce.
    /// </summary>
    IReadOnlyList<string> TargetTypes { get; }

    /// <summary>
    /// Checks if the pipeline can convert the specified source type to the specified target type.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type.</param>
    /// <returns><c>true</c> of a type can be processed.</returns>
    bool CanConvert(string sourceType, string targetType);

    /// <summary>
    /// Converts the source to the target type.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="options">The compilation options.</param>
    /// <returns>The <see cref="PipelineResult"/>.</returns>
    PipelineResult Convert(object source, object? options);
}