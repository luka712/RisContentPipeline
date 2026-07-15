namespace RisContentPipeline;

/// <summary>
/// The state of queued items.
/// </summary>
public enum QueuedItemState
{
    Queued,
    Processing,
    Done,
    Failed
}