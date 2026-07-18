namespace RisContentPipeline;

/// <summary>
/// Represents a single item sitting in the pipeline queue, tracking its
/// conversion lifecycle and the final <see cref="PipelineResult"/>.
/// </summary>
public class QueuedPipelineItem
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="item">The <see cref="PipelineConversionItem"/>.</param>
    public QueuedPipelineItem(PipelineConversionItem item)
    {
        Item = item;
    }

    /// <summary>
    /// The event fired when the conversion starts.
    /// </summary>
    public event EventHandler? OnConversionStarted;

    /// <summary>
    /// The event fired when the conversion finishes.
    /// </summary>
    public event EventHandler? OnConversionFinished;
    
    /// <summary>
    /// The event fired when the conversion fails.
    /// </summary>
    public event EventHandler? OnConversionFailed;

    /// <summary>
    /// The item to be processed.
    /// </summary>
    public PipelineConversionItem Item { get; }

    /// <summary>
    /// The result of the conversion, if it has completed.
    /// </summary>
    public PipelineResult? Result { get; internal set; }

    /// <summary>
    /// The state of the item.
    /// </summary>
    public QueuedItemState State { get; private set; } = QueuedItemState.Queued;

    /// <summary>
    /// Called when the conversion starts.
    /// </summary>
    internal void ConversionStart()
    {
        State = QueuedItemState.Processing;
        OnConversionStarted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the conversion finishes.
    /// </summary>
    internal void ConversionFinished(PipelineResult result)
    {
        Result = result;
        State = QueuedItemState.Done;
        OnConversionFinished?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the conversion fails.
    /// </summary>
    internal void ConversionFailed(PipelineResult result)
    {
        Result = result;
        State = QueuedItemState.Failed;
        OnConversionFailed?.Invoke(this, EventArgs.Empty);       
    }
}
