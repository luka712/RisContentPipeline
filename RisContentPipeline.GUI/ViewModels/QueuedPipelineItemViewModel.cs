using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.ViewModels;

/// <summary>
/// The view model for a queued pipeline item.
/// </summary>
public partial class QueuedPipelineItemViewModel : ViewModelBase
{
    private readonly ViewerService _viewerService;
    private readonly MessageService _messageService;

    [ObservableProperty] private bool _inProgress;

    [ObservableProperty] private bool _isFinished;

    [ObservableProperty] public bool _inError;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="item">The <see cref="QueuedPipelineItem"/>.</param>
    /// <param name="preferences">The <see cref="PreferencesViewModel"/>.</param>
    public QueuedPipelineItemViewModel(QueuedPipelineItem item, PreferencesViewModel preferences)
    {
        _viewerService = App.Services.GetService<ViewerService>()!;
        _messageService = App.Services.GetService<MessageService>()!;

        Item = item;
        Preferences = preferences;

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

        Item.OnConversionStarted += (_, _) => ItemStatusChanged();
        Item.OnConversionFinished += (_, _) => ItemStatusChanged();
    }
    
    /// <summary>
    /// The <see cref="QueuedPipelineItem"/>.
    /// </summary>
    public QueuedPipelineItem Item { get; }


    /// <summary>
    /// The preferences.
    /// </summary>
    public PreferencesViewModel Preferences { get; set; }
    
    private void ItemStatusChanged()
    {
        IsFinished = Item.State == QueuedItemState.Done;
        InProgress = Item.State == QueuedItemState.Processing;
        InError = Item.State == QueuedItemState.Failed;
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
        _viewerService.ViewAsset(this);
    }
}