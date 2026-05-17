using RisContentPipeline.Ktx2;

namespace RisContentPipeline;

/// <summary>
/// The default <see cref="IPipelineSystem"/> implementation.
/// Maintains a registry of <see cref="IPipeline"/> instances and a queue of source assets
/// that should be processed when <see cref="ConvertAllAsync"/> is invoked.
/// </summary>
public class PipelineSystem : IPipelineSystem
{
    /// <summary>
    /// The store for the source and target types and the source and options.
    /// Each tuple consists of (sourceType, targetType, source, options).
    /// </summary>
    private readonly List<QueuedPipelineItem> _store = new();

    private readonly object _storeLock = new();

    private readonly List<IPipeline> _pipelines =
    [
        new Ktx2Pipeline()
    ];

    /// <inheritdoc/>
    public IReadOnlyList<QueuedPipelineItem> QueuedItems
    {
        get
        {
            lock (_storeLock)
            {
                return _store.ToList();
            }
        }
    }

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
    public QueuedPipelineItem StoreSourceAsset(string sourceType, string targetType, object source, object? options)
    {
        var queuedItem = new QueuedPipelineItem(new()
        {
            SourceType = sourceType,
            TargetType = targetType,
            Source = source,
            Options = options
        });

        lock (_storeLock)
        {
            _store.Add(queuedItem);
        }

        return queuedItem;
    }

    private static void Validate(PipelineConversionItem item)
    {
        ArgumentNullException.ThrowIfNull(item.SourceType);
        ArgumentNullException.ThrowIfNull(item.TargetType);
        ArgumentNullException.ThrowIfNull(item.Source);
    }

    /// <inheritdoc/>
    public QueuedPipelineItem StoreSourceAsset(PipelineConversionItem item)
    {
        Validate(item);
        return StoreSourceAsset(item.SourceType, item.TargetType, item.Source, item.Options);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<PipelineResult>> ConvertAllAsync()
    {
        List<QueuedPipelineItem> snapshot;
        lock (_storeLock)
        {
            snapshot = _store.ToList();
        }

        OnConvertAllStarted?.Invoke(this, new ConvertStartEventArgs() { TotalItems = snapshot.Count });

        var tasks = new List<Task<PipelineResult>>();
        foreach (var queuedItem in snapshot)
        {
            tasks.Add(Task.Run(() =>
            {
                queuedItem.ConversionStart();
                var result = Convert(queuedItem.Item);
                queuedItem.Result = result;
                queuedItem.ConversionFinished();
                OnItemConversionFinish?.Invoke(this, new OnItemConversionFinishEventArgs() { Item = queuedItem, Result = result });
                return result;
            }));
        }

        await Task.WhenAll(tasks);

        OnConvertAllFinished?.Invoke(this, new ConvertEndedEventArgs() { TotalItems = snapshot.Count });

        return tasks.Select(x => x.Result).ToList();
    }

    /// <inheritdoc/>
    public PipelineResult Convert(string sourceType, string targetType, object source, object? options)
    {
        foreach (var pipeline in _pipelines)
        {
            if (pipeline.CanConvert(sourceType, targetType))
            {
                return pipeline.Convert(source, options);
            }
        }

        return PipelineResult.FailureResult($"No pipeline registered that can convert '{sourceType}' to '{targetType}'.");
    }

    /// <inheritdoc/>
    public PipelineResult Convert(PipelineConversionItem item)
    {
        Validate(item);
        return Convert(item.SourceType, item.TargetType, item.Source, item.Options);
    }

    /// <inheritdoc/>
    public void ClearStoredAssets()
    {
        lock (_storeLock)
        {
            _store.Clear();
        }
    }
}
