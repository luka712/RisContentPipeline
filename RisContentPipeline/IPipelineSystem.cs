namespace RisContentPipeline;

/// <summary>
/// Event arguments fired when the conversion of all stored sources begins.
/// </summary>
public class ConvertStartEventArgs
{
    /// <summary>
    /// The total number of items that will be converted.
    /// This is useful for progress reporting.
    /// </summary>
    public int TotalItems { get; internal set; }
}

/// <summary>
/// Event arguments fired after a single item finished its conversion.
/// </summary>
public class OnItemConversionFinishEventArgs
{
    /// <summary>
    /// The queued item that was converted.
    /// </summary>
    public QueuedPipelineItem Item { get; internal set; } = null!;

    /// <summary>
    /// The result of the conversion of the item.
    /// </summary>
    public PipelineResult Result { get; internal set; } = new();
}

/// <summary>
/// Event arguments fired after every stored source has been processed.
/// </summary>
public class ConvertEndedEventArgs
{
    /// <summary>
    /// The total number of items that were converted.
    /// This is useful for progress reporting.
    /// </summary>
    public int TotalItems { get; internal set; }
}

/// <summary>
/// The pipeline system.
/// </summary>
public interface IPipelineSystem
{
    /// <summary>
    /// The items currently queued for conversion.
    /// </summary>
    IReadOnlyList<QueuedPipelineItem> QueuedItems { get; }
    
    /// <summary>
    /// The event that is called when the conversion of all stored sources is started.
    /// </summary>
    event EventHandler<ConvertStartEventArgs>? OnConvertAllStarted;

    /// <summary>
    /// The event that is called when the conversion of all stored sources is finished.
    /// </summary>
    event EventHandler<ConvertEndedEventArgs>? OnConvertAllFinished;

    /// <summary>
    /// Called after each individual stored source has been processed during a <see cref="ConvertAll"/> run.
    /// The event handler receives the <see cref="PipelineResult"/> of the item that was just converted.
    /// </summary>
    event EventHandler<OnItemConversionFinishEventArgs>? OnItemConversionFinish;

    /// <summary>
    /// Add a pipeline to the pipeline system.
    /// </summary>
    /// <param name="pipeline">The <see cref="IPipeline"/>.</param>
    void AddPipeline(IPipeline pipeline);
    
    /// <summary>
    /// Store a source for later processing.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type. Add '*' to accept any target type.</param>
    /// <param name="source">The source object.</param>
    /// <param name="options">The options.</param>
    QueuedPipelineItem StoreSourceAsset(string sourceType, string targetType, object source, object? options);
    
    /// <summary>
    /// Store item for later processing.
    /// </summary>
    /// <param name="item">The <see cref="PipelineConversionItem"/>.</param>
    QueuedPipelineItem StoreSourceAsset(PipelineConversionItem item);
    
    /// <summary>
    /// Convert all stored sources to targets.
    /// </summary>
    /// <returns>The list of all results.</returns>
    Task<IReadOnlyList<PipelineResult>> ConvertAllAsync();

    /// <summary>
    /// Convert a single source to a target.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type. Add '*' to accept any target type.</param>
    /// <param name="source">The source.</param>
    /// <param name="options">The compilation options.</param>
    PipelineResult Convert(string sourceType, string targetType, object source, object? options);
    
    /// <summary>
    /// Convert a single source to a target.
    /// </summary>
    /// <param name="item">The <see cref="PipelineConversionItem"/>.</param>
    /// <returns>The <see cref="PipelineResult"/>.</returns>
    PipelineResult Convert(PipelineConversionItem item);    

    /// <summary>
    /// Removes all stored source assets without affecting registered pipelines.
    /// Useful when re-building from scratch.
    /// </summary>
    void ClearStoredAssets();
}
