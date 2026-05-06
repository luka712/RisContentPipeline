using RisContentPipeline.Ktx2;

namespace RisContentPipeline;

public class PipelineSystem : IPipelineSystem
{
    /// <summary>
    /// The store for the source and target types and the source and options.
    /// </summary>
    private List<Tuple<string, string, object, object?>> _store = new();
    
    private readonly List<IPipeline> _pipelines =
    [
        new Ktx2Pipeline()
    ];

    /// <summary>
    /// Add a pipeline to the content pipelines.
    /// </summary>
    /// <param name="pipeline">The pipeline to add.</param>
    public void RegisterPipeline(IPipeline pipeline)
    {
        _pipelines.Add(pipeline);
    }

    /// <inheritdoc/>
    public void Store(string sourceType, string targetType, object source, object? options)
    {
        _store.Add(Tuple.Create(sourceType, targetType, source, options));
    }

    /// <inheritdoc/>
    public IReadOnlyList<PipelineResult> ConvertAll()
    {
        var results = new List<PipelineResult>();
        foreach(var (sourceType, targetType, source, options) in _store)
        {
            results.Add(Convert(sourceType, targetType, source, options));
        }
        return results;
    }

    /// <summary>
    /// Convert the source to the target.
    /// </summary>
    public PipelineResult Convert(string sourceType, string targetType, object source, object? options)
    {
        foreach (var pipeline in _pipelines)
        {
            if (pipeline.CanConvert(sourceType, targetType))
            {
                return pipeline.Convert(source, options);
            }
        }

        return new PipelineResult();
    }
}