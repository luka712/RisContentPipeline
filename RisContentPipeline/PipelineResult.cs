namespace RisContentPipeline;

/// <summary>
/// The result of a pipeline execution.
/// </summary>
public class PipelineResult
{
    /// <summary>
    /// The result of the pipeline execution.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The resulting object of the pipeline execution.
    /// </summary>
    public object? Result { get; set; }
    
    /// <summary>
    /// The error message if the pipeline execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}