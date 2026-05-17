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
    /// The item to be processed.
    /// </summary>
    public PipelineConversionItem Item { get; }

    /// <summary>
    /// The result of the conversion, if it has completed.
    /// </summary>
    public PipelineResult? Result { get; internal set; }

    /// <summary>
    /// The conversion is in progress.
    /// </summary>
    public bool IsStarted { get; private set; }

    /// <summary>
    /// True if the item has been processed.
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// Called when the conversion starts.
    /// </summary>
    internal void ConversionStart()
    {
        IsStarted = true;
        OnConversionStarted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the conversion finishes.
    /// </summary>
    internal void ConversionFinished()
    {
        IsFinished = true;
        OnConversionFinished?.Invoke(this, EventArgs.Empty);
    }
}
