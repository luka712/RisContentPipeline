using System;
using System.Collections.Generic;
using System.Text;

namespace RisContentPipeline.Generic
{
    /// <summary>
    /// The <see cref="GenericPipelineOptions"/> class is a placeholder for any options or settings that may be needed for the generic content pipeline.
    /// </summary>
    public class GenericPipelineOptions
    {
        /// <summary>
        /// The output path where the processed content should be saved. 
        /// This can be used by the content pipeline to determine where to write the output files after processing.
        /// </summary>
        public string? OutputPath { get; set; }
    }
}
