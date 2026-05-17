using Eto.Forms;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Windows;
using RisContentPipeline.Ktx2;
using RisKtx2;

namespace RisContentPipeline.GUI.Views.Inspector;

/// <summary>
/// Displays the list of queued converter jobs and their current status
/// inside the Inspector tab.
/// </summary>
internal class BuildView
{
    private const int SOURCE_COLUMN = 1;
    private const int TARGET_COLUMN = 5;
    
    private readonly Context _context;
    private readonly TreeGridView _treeView;
    private readonly TreeGridItem _rootItem;
    private readonly HashSet<QueuedPipelineItem> _wiredItems = new();

    /// <summary>
    /// The content panel.
    /// </summary>
    public Panel Content { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildView"/> class.
    /// </summary>
    /// <param name="context">The application context.</param>
    internal BuildView(Context context)
    {
        _context = context;

        _treeView = new TreeGridView
        {
            ShowHeader = true,
            AllowMultipleSelection = false,
            Border = BorderType.None,
        };

        _treeView.CellDoubleClick += OnCellDoubleClick;

        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Source",
            AutoSize = true,
            DataCell = new ImageTextCell(0, SOURCE_COLUMN),
        });
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Conversion",
            Width = 100,
            DataCell = new TextBoxCell(2),
        });
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Status",
            Width = 90,
            DataCell = new TextBoxCell(3),
        });
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Target",
            AutoSize = true,
            DataCell = new ImageTextCell(4,TARGET_COLUMN),
        });

        _rootItem = new TreeGridItem
        {
            Values = [Icons.FolderIcon, "Converter Jobs", "", ""],
            Expanded = true,
        };

        _treeView.DataStore = _rootItem;

        WireSystemEvents();

        Content = new Panel
        {
            Padding = Theme.PADDING,
            Content = _treeView,
        };
    }
    
    /// <summary>
    /// Invoked when a target is selected.
    /// </summary>
    internal event Action<object> OnTargetSelected; 

    private void WireSystemEvents()
    {
        _context.PipelineSystem.OnConvertAllStarted += (s, e) =>
            _context.AsyncInvoke(Refresh);
        _context.PipelineSystem.OnItemConversionFinish += (s, e) =>
            _context.AsyncInvoke(Refresh);
        _context.PipelineSystem.OnConvertAllFinished += (s, e) =>
            _context.AsyncInvoke(Refresh);
    }

    private void Refresh()
    {
        _rootItem.Children.Clear();

        foreach (var queuedItem in _context.PipelineSystem.QueuedItems)
        {
            WireItemEvents(queuedItem);

            var sourcePath = GetSourceDisplay(queuedItem.Item.Source);
            var conversion = $"{queuedItem.Item.SourceType} → {queuedItem.Item.TargetType}";
            var status = GetStatusText(queuedItem);
            var targetPath = GetTargetDisplay(queuedItem);

            _rootItem.Children.Add(new TreeGridItem
            {
                Values = [Icons.FileIcon, sourcePath, conversion, status, Icons.FileIcon, targetPath],
            });
        }

        _treeView.ReloadData();
    }

    private void WireItemEvents(QueuedPipelineItem item)
    {
        if (!_wiredItems.Add(item))
            return;

        item.OnConversionStarted += (s, e) => _context.AsyncInvoke(Refresh);
        item.OnConversionFinished += (s, e) => _context.AsyncInvoke(Refresh);
    }

    private static string GetSourceDisplay(object source)
    {
        if (source is Ktx2PipelineSource ktx2Source)
            return Path.GetFileName(ktx2Source.FilePath);
        if (source is GenericPipelineSource genericSource)
            return Path.GetFileName(genericSource.FilePath);
        if (source is string str)
            return str;

        return source.ToString() ?? "Unknown";
    }
    
    private static string GetTargetDisplay(QueuedPipelineItem pipelineQueue)
    {
        var result = pipelineQueue.Result;
        
        if (result?.Success != true)
        {
            return "";
        }

        if (pipelineQueue.Item.Options is Ktx2PipelineOptions ktx2Options)
        {
            var filePath = Path.GetFileName(ktx2Options.OutputPath);
            return filePath;
        }

        throw new NotImplementedException();
    }

    private static string GetStatusText(QueuedPipelineItem item)
    {
        if (item.IsFinished)
        {
            if (item.Result?.Success == true)
                return "Done";
            return "Error";
        }

        if (item.IsStarted)
        {
            return "Converting...";
        }

        return "Pending";
    }

    private void OnCellDoubleClick(object? cell, GridCellMouseEventArgs gridCellMouseEventArgs)
    {
        var treeGridItem = (TreeGridItem)gridCellMouseEventArgs.Item;
        var source = treeGridItem.Values[SOURCE_COLUMN];
        var target = treeGridItem.Values[TARGET_COLUMN];
        
        // If it's target double-click.
        if (gridCellMouseEventArgs.Column == 3 || gridCellMouseEventArgs.Column == 4 || gridCellMouseEventArgs.Column == 5)
        {
            if (!String.IsNullOrEmpty(target.ToString()))
            {
                var imageViewer = new ImageViewerWindow(_context);
                string filePath = Path.Combine(AppContext.BaseDirectory, "Build", target.ToString());
                imageViewer.Show();
                imageViewer.FilePath = filePath;
            }
        }
    }
}
