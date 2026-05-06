namespace RisContentPipeline;

/// <summary>
/// The pipeline system.
/// </summary>
public interface IPipelineSystem
{
    /// <summary>
    /// Store a source for later processing.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="source">The source object.</param>
    /// <param name="options">The options.</param>
    void Store(string sourceType, string targetType, object source, object? options);
    
    /// <summary>
    /// Convert all stored sources to targets.
    /// </summary>
    /// <returns>The list of all results.</returns>
    IReadOnlyList<PipelineResult> ConvertAll();
    
    /// <summary>
    /// Convert a single source to a target.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="source">The source.</param>
    /// <param name="options">The compilation options.</param>
    PipelineResult Convert(string sourceType, string targetType, object source, object? options);
}