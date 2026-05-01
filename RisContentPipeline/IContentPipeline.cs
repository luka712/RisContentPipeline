namespace RisContentPipeline
{
    /// <summary>
    /// Pipeline interface for processing content from a source type to a destination type. 
    /// This is the core interface that all content processing pipelines must implement.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    internal interface IContentPipeline<TSource, TDestination>
    {
        /// <summary>
        /// Processes the content from the source type to the destination type.
        /// This method should contain the logic for transforming the input data into the desired output format.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options for processing, which can be used to customize the behavior of the pipeline based on specific requirements or settings.</param>
        /// <returns>The destination.</returns>
        TDestination Convert(TSource source, object? options = null);
    }
}
