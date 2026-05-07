using RisContentPipeline.Ktx2;

namespace RisContentPipeline;

// TODO: Add doc comments.
public class PipelineSystem : IPipelineSystem
{
    /// <summary>
    /// The store for the source and target types and the source and options.
    /// </summary>
    private readonly List<Tuple<string, string, object, object?>> _store = new();

    private readonly List<IPipeline> _pipelines =
    [
        new Ktx2Pipeline()
    ];

    /// <inheritdoc/>
    public event EventHandler<ConvertStartEventArgs>? OnConvertAllStarted;

    /// <inheritdoc/>
    public event EventHandler<ConvertEndedEventArgs>? OnConvertAllFinished;

    /// <inheritdoc/>
    public event EventHandler<OnItemConversionFinishEventArgs>? OnItemConversionFinish;

    /// <inheritdoc/>
    public void AddPipeline(IPipeline pipeline)
    {
        // If the pipeline is already added, do nothing.
        if (_pipelines.Any(x => x.Name == pipeline.Name))
        {
            return;
        }
        _pipelines.Add(pipeline);
    }

    /// <inheritdoc/>
    public void StoreSourceAsset(string sourceType, string targetType, object source, object? options)
    {
        _store.Add(Tuple.Create(sourceType, targetType, source, options));
    }

    /// <inheritdoc/>
    public IReadOnlyList<PipelineResult> ConvertAll()
    {
        OnConvertAllStarted?.Invoke(this, new ConvertStartEventArgs() { TotalItems = _store.Count });

        var results = new List<PipelineResult>();
        foreach (var (sourceType, targetType, source, options) in _store)
        {
            var result = Convert(sourceType, targetType, source, options);
            OnItemConversionFinish?.Invoke(this, new OnItemConversionFinishEventArgs() { Result = result! });
            results.Add(result);
        }

        OnConvertAllFinished?.Invoke(this, new ConvertEndedEventArgs() { TotalItems = _store.Count });

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