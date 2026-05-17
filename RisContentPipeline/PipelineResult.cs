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

    /// <summary>
    /// Creates a successful pipeline result.
    /// </summary>
    /// <param name="result">The optional resulting object.</param>
    /// <returns>A successful <see cref="PipelineResult"/>.</returns>
    public static PipelineResult SuccessResult(object? result = null) => new() { Success = true, Result = result };

    /// <summary>
    /// Creates a failed pipeline result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed <see cref="PipelineResult"/>.</returns>
    public static PipelineResult FailureResult(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
}
