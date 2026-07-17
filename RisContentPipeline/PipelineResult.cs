namespace RisContentPipeline;

    /// <summary>
    /// The result of a pipeline execution. Immutable after creation for thread-safety in async scenarios.
    /// </summary>
    public class PipelineResult
    {
        /// <summary>
        /// Whether the pipeline execution succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// The resulting object of the pipeline execution (e.g. Ktx2Texture).
        /// </summary>
        public object? Result { get; }

        /// <summary>
        /// The error message if the pipeline execution failed.
        /// </summary>
        public string? ErrorMessage { get; }

        protected PipelineResult(bool success, object? result = null, string? errorMessage = null)
        {
            Success = success;
            Result = result;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a successful pipeline result.
        /// </summary>
        /// <param name="result">The optional resulting object.</param>
        /// <returns>A successful <see cref="PipelineResult"/>.</returns>
        public static PipelineResult SuccessResult(object? result = null) => new(true, result);

        /// <summary>
        /// Creates a failed pipeline result.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A failed <see cref="PipelineResult"/>.</returns>
        public static PipelineResult FailureResult(string errorMessage) => new(false, errorMessage: errorMessage);
    }
