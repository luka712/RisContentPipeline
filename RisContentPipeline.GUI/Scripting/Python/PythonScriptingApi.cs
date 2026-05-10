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

        /// <summary>
        /// Creates a new <see cref="PythonScriptingApi"/> bound to the supplied pipeline system.
        /// Every method exposed on this class becomes available to Python scripts as the
        /// <c>api</c> module-level attribute that the host injects into each script.
        /// </summary>
        /// <param name="pipelineSystem">The pipeline system that newly-registered pipelines will be added to.</param>
        public PythonScriptingApi(IPipelineSystem pipelineSystem)
        {
            _pipelineSystem = pipelineSystem;
        }

        /// <summary>
        /// The current file or folder being processed in the content pipeline.
        /// This is set by the content pipeline before executing the Python script, allowing the script to access information about the file or folder being processed.
        /// </summary>
        public PythonAssetFile? current_asset { get; internal set; }

        /// <summary>
        /// Validates that <paramref name="json"/> is a JSON object and writes it to <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The destination file path.</param>
        /// <param name="json">The JSON string to write. Must represent a JSON object.</param>
        /// <returns><c>true</c> if the file was written successfully, otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Reads the file at <paramref name="path"/> and returns its raw text content.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>The file content as a string, or an empty string if the file cannot be read.</returns>
        public string read_json_as_str(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read JSON file: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Reads the file at <paramref name="path"/> and deserializes it into a dictionary.
        /// </summary>
        /// <param name="path">The path of the JSON file to read.</param>
        /// <returns>A dictionary representing the parsed JSON object, or an empty dictionary if the file cannot be parsed.</returns>
        public Dictionary<string, object> read_json(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Internal helper that creates a <see cref="GenericPipeline"/> wrapping the given
        /// managed callback and registers it with the pipeline system.
        /// </summary>
        /// <param name="name">The unique pipeline name.</param>
        /// <param name="source">The list of supported source types.</param>
        /// <param name="target">The list of supported target types. Use <c>"*"</c> to accept any.</param>
        /// <param name="convertAction">The conversion callback returning a result dictionary
        /// with <c>success</c>, <c>message</c>, and <c>result</c> keys.</param>
        /// <returns>The newly-registered <see cref="IPipeline"/>.</returns>
        private IPipeline AddPipeline(
            string name,
            string[] source,
            string[] target,
            Func<object, object?, Dictionary<string, object>> convertAction)
        {
            PipelineResult Callback(object obj, object? opt)
            {
                if (obj is GenericPipelineSource genericObj)
                {
                    var pyObj = PyObject.FromManagedObject(genericObj);
                    pyObj.SetAttr("file_path", PyObject.FromManagedObject(genericObj.FilePath));
                    obj = pyObj;

                }
                if (opt is GenericPipelineOptions genericOpt)
                {
                    var pyOpt = PyObject.FromManagedObject(genericOpt);
                    pyOpt.SetAttr("output_path", PyObject.FromManagedObject(genericOpt.OutputPath));
                    opt = pyOpt;
                }

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

        /// <summary>
        /// Registers a Python-defined pipeline with the underlying <see cref="IPipelineSystem"/>.
        /// The Python function is expected to take a source object and an options object and to
        /// return a dictionary with the keys <c>success</c> (bool), <c>message</c> (str, optional),
        /// and <c>result</c> (any, optional).
        /// </summary>
        /// <param name="name">The unique pipeline name.</param>
        /// <param name="source">The list of supported source types.</param>
        /// <param name="target">The list of supported target types. Use <c>"*"</c> to accept any.</param>
        /// <param name="pyFunction">The Python callable that performs the conversion.</param>
        /// <returns>The newly-registered <see cref="IPipeline"/>.</returns>
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
            return AddPipeline(name, source, target, ConvertActionWrapper);
        }
    }
}