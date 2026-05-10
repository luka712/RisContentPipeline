namespace RisContentPipeline.Generic
{
    /// <summary>
    /// The <see cref="GenericPipelineSource"/> class represents a source file or folder in the content pipeline. 
    /// It contains information about the file path and other relevant details that can be used by the 
    /// content pipeline to process the source during the build process.
    /// </summary>
    public record GenericPipelineSource
    {
        /// <summary>
        /// The file path of the source file or folder. This is the path that the content pipeline will use to locate the source during processing.
        /// </summary>
        public required string FilePath { get; set; }
    }
}
