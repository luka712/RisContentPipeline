using Eto.Drawing;
using Eto.Forms;
using Python.Runtime;

namespace RisContentPipeline.GUI.Views;

public class StatusBar
{
    private readonly Context _context;
    private readonly Label _statusLabel = new() { Text = "Ready" };

    private readonly StackLayout _progressBarLayout;
    private readonly ProgressBar _progressBar = new()
    {
        MinValue = 0,
        MaxValue = 100,
        Indeterminate = true,
    };
    private readonly Label _progressBarLabel = new();

    private int _itemsCount;
    private int _itemsProcessed;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="context">The <see cref="Context"/>.</param>
    internal StatusBar(Context context)
    {
        _context = context;
        
        _progressBarLayout = new StackLayout()
        {
            Orientation = Orientation.Horizontal,
            Visible = false,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            Items =
            {
                _progressBarLabel,
                _progressBar
            }
        };

        Panel = new TableLayout()
        {
            BackgroundColor = SystemColors.ControlBackground,
            Padding = new Padding(8, 4),
            Rows =
            {
                new TableRow(_statusLabel, null, _progressBarLayout)
            }
        };

        WireBuildLoggerToStatus();

        _context.OnBuildStarted += ShowProgressBar;

        _context.PipelineSystem.OnConvertAllStarted += (_, args) =>
        {
            _itemsProcessed = 0;
            _itemsCount = args.TotalItems;

            Application.Instance.Invoke(() =>
            {
                _progressBarLabel.Text = "Building 0%  ";
                _progressBar.Indeterminate = _itemsCount <= 1;
            });
        };

        _context.PipelineSystem.OnItemConversionFinish += (_, _) =>
        {
            _itemsProcessed++;
            Application.Instance.Invoke(() =>
            {
                var progress = (int)(_itemsProcessed / (double)_itemsCount * 100);
                _progressBarLabel.Text = $"Building {progress}%  ";
                _progressBar.Value = progress;
            });
        };

        _context.PipelineSystem.OnConvertAllFinished += (_, _) =>
        {
            Application.Instance.Invoke(() =>
            {
                _progressBarLabel.Text = "Build Finished  ";
                var progress = (int)(_itemsProcessed / (double)_itemsCount * 100);
                _progressBar.Value = progress;
            });
        };
    }

    /// <summary>
    /// Mirrors interesting <see cref="MessageLogger"/> events into the status bar so the
    /// user always has a quick read-out of the last action.
    /// </summary>
    private void WireBuildLoggerToStatus()
    {
        var logger = _context.MessageLogger;
        logger.OnInfoLog += msg => UpdateStatus(msg);
        logger.OnSuccessLog += msg => UpdateStatus(msg);
        logger.OnErrorLog += msg => UpdateStatus($"Error: {msg}");
    }

    public TableLayout Panel { get; }

    /// <summary>
    /// Shows the progress bar.
    /// </summary>
    private void ShowProgressBar()
    {
        _progressBarLayout.Visible = true;
    }

    /// <summary>
    /// Hides the progress bar.
    /// </summary>
    private void HideProgressBar()
    {
        _progressBarLayout.Visible = false;
    }

    /// <summary>
    /// Updates the status bar.
    /// </summary>
    /// <param name="message">The message.</param>
    public void UpdateStatus(string message)
    {
        if (_statusLabel != null)
        {
            _statusLabel.Text = message;
        }
    }
}