using System.Text.Json;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Scripting.Python.Data;

namespace RisContentPipeline.GUI.Scripting.Python
{
    /// <summary>
    /// The <see cref="PythonScriptingApi"/> class provides an API for Python scripts to interact with the content pipeline.
    /// </summary>
    public class PythonScriptingApi
    {
        private readonly IPipelineSystem _pipelineSystem;

        // TODO: create a doc comment for this constructor
        public PythonScriptingApi(IPipelineSystem pipelineSystem)
        {
            _pipelineSystem = pipelineSystem;
        }
        
        /// <summary>
        /// The current file or folder being processed in the content pipeline.
        /// This is set by the content pipeline before executing the Python script, allowing the script to access information about the file or folder being processed.
        /// </summary>
        public PythonAssetFile? current_asset { get; internal set; }

        // TODO: create a doc comment for this method
        public bool save_json(string path, string json)
        {
            try
            {
                bool isJson = JsonDocument.Parse(json).RootElement.ValueKind == JsonValueKind.Object;
                if (!isJson)
                {
                    return false;
                }

                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write JSON file: {ex.Message}");
                return false;
            }
        }

        // TODO: create a doc comment for this method
        public Dictionary<string, object> read_json(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        // TODO: create a doc comment for this method
        public IPipeline add_pipeline(string name, string[] source, string[] target,
            Func<object, object?, Dictionary<string, object>> convertAction)
        {
            PipelineResult Callback(object obj, object? opt)
            {
                var dict = convertAction(obj, opt);

                dict.TryGetValue("success", out object? success);
                dict.TryGetValue("message", out object? message);
                dict.TryGetValue("result", out object? result);

                return new PipelineResult() { Success = success is bool and true, ErrorMessage = message is string and { Length: > 0 } ? message.ToString() : null, Result = result };
            }

            var pipeline = new GenericPipeline(name, source, target, Callback);
            _pipelineSystem.AddPipeline(pipeline);
            return pipeline;
        }
    }
}