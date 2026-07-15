using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for a queued pipeline item.
/// </summary>
public partial class QueuedPipelineItemViewModel : ViewModelBase
{
    private readonly QueuedPipelineItem _item;

    [ObservableProperty] private bool _inProgress;

    [ObservableProperty] private bool _isFinished;

    [ObservableProperty] public bool _inError;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="item">The <see cref="QueuedPipelineItem"/>.</param>
    public QueuedPipelineItemViewModel(QueuedPipelineItem item)
    {
        _item = item;

        if (item.Item.Source is Ktx2PipelineSource ktxSource)
        {
            var options = item.Item.Options as Ktx2PipelineOptions;

            Source = Path.GetFileName(ktxSource.FilePath);

            if (!string.IsNullOrEmpty(options?.OutputPath))
            {
                Target = Path.GetFileName(options.OutputPath);
            }

            Conversion = "PNG -> KTX2";
        }
        else
        {
            throw new NotImplementedException();
        }

        _item.OnConversionStarted += (_, _) => ItemStatusChanged();
        _item.OnConversionFinished += (_, _) => ItemStatusChanged();
    }

    private void ItemStatusChanged()
    {
        IsFinished = _item.State == QueuedItemState.Done;
        InProgress = _item.State == QueuedItemState.Processing;
        InError = _item.State == QueuedItemState.Failed;
    }

    /// <summary>
    /// The source path of the item.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// The target path of the item.
    /// </summary>
    public string? Target { get; }

    /// <summary>
    /// The conversion type of the item.
    /// </summary>
    public string? Conversion { get; }

    [RelayCommand]
    public void ViewSelf()
    {
    }
}