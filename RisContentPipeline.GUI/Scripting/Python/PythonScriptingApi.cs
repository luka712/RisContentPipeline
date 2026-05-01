using RisContentPipeline.GUI.Scripting.Python.Data;

namespace RisContentPipeline.GUI.Scripting.Python
{
    /// <summary>
    /// The <see cref="PythonScriptingApi"/> class provides an API for Python scripts to interact with the content pipeline.
    /// </summary>
    public class PythonScriptingApi
    {
        /// <summary>
        /// The current file or folder being processed in the content pipeline.
        /// This is set by the content pipeline before executing the Python script, allowing the script to access information about the file or folder being processed.
        /// </summary>
        public PythonAssetFile? current_asset { get; internal set; }
    }
}
