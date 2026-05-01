using System;
using System.Collections.Generic;
using System.Text;

namespace RisContentPipeline.Container
{
    internal class GenericContainer
    {
        /// <summary>
        /// Generic container for files that are not processed by the content pipeline but still need to be included in the output.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// The contents of the file as a string. This can be used for text files or any other file type that can be represented as a string. For binary files, this should be left empty and the file should be copied directly to the output directory.
        /// </summary>
        public string FileContents { get; set; } = string.Empty;
    }
}
