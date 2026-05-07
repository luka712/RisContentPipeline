namespace RisContentPipeline;

public class ConvertStartEventArgs
{
    /// <summary>
    /// The total number of items that will be converted.
    /// This is useful for progress reporting.
    /// </summary>
    public int TotalItems { get; internal set; }
}

public class OnItemConversionFinishEventArgs
{
    /// <summary>
    /// The result of the conversion of the item.
    /// </summary>
    public PipelineResult Result { get; internal set; }
}

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
    /// The event that is called when the conversion of all stored sources is started.
    /// </summary>
    event EventHandler<ConvertStartEventArgs>? OnConvertAllStarted;

    /// <summary>
    /// The event that is called when the conversion of all stored sources is finished.
    /// </summary>
    event EventHandler<ConvertEndedEventArgs>? OnConvertAllFinished;

    /// <summary>
    /// Called when a pipeline item is run. The event handler receives the <see cref="PipelineResult"/> of the item that was run.
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
    void StoreSourceAsset(string sourceType, string targetType, object source, object? options);
    
    /// <summary>
    /// Convert all stored sources to targets.
    /// </summary>
    /// <returns>The list of all results.</returns>
    IReadOnlyList<PipelineResult> ConvertAll();

    /// <summary>
    /// Convert a single source to a target.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type. Add '*' to accept any target type.</param>
    /// <param name="source">The source.</param>
    /// <param name="options">The compilation options.</param>
    PipelineResult Convert(string sourceType, string targetType, object source, object? options);
}