using System.Text.Json;
using Python.Runtime;
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
        private void AddPipeline(
            string name,
            string[] source,
            string[] target,
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

            _pipelineSystem.AddPipeline(new GenericPipeline(name, source, target, Callback));
        }

        // TODO: create a doc comment for this method
        public IPipeline add_pipeline(string name, string[] source, string[] target, dynamic pyFunction)
        {
            // Wrapper that converts PyObject function calls to the expected delegate signature
            Dictionary<string, object> ConvertActionWrapper(object obj, object? opt)
            {
                using (Py.GIL())
                {
                    try
                    {
                        // Call the Python function with the provided arguments
                        dynamic result = pyFunction(obj, opt);

                        // Convert the Python dictionary result to C# Dictionary
                        var dict = new Dictionary<string, object>();

                        if (result is PyObject pyObj)
                        {
                            // Use Python.NET's built-in dictionary conversion
                            dynamic pyDict = pyObj;

                            // Get the items() method from the Python dict
                            foreach (PyObject item in pyDict.items())
                            {
                                using (item)
                                {
                                    // Each item is a tuple (key, value)
                                    string key = item[0].As<string>();
                                    object value = item[1].AsManagedObject(typeof(object));
                                    dict[key] = value;
                                }
                            }
                        }
                        else if (result != null)
                        {
                            // Handle case where result is already converted
                            var resultType = result.GetType();
                            if (resultType.IsGenericType &&
                                resultType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                            {
                                foreach (var key in result.Keys)
                                {
                                    dict[key.ToString()] = result[key];
                                }
                            }
                        }

                        return dict;
                    }
                    catch (Exception ex)
                    {
                        // Return error result if Python function fails
                        return new Dictionary<string, object>
                        {
                            { "success", false },
                            { "message", $"Python function error: {ex.Message}" }
                        };
                    }
                }
            }

            // Use the existing add_pipeline method with the wrapper
            return add_pipeline(name, source, target, ConvertActionWrapper);
        }
    }
}