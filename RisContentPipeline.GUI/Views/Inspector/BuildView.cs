using Eto.Forms;
using RisContentPipeline.Generic;
using RisContentPipeline.GUI.Windows;
using RisContentPipeline.Ktx2;
using RisKtx2;
using System.Dynamic;

namespace RisContentPipeline.GUI.Views.Inspector;

/// <summary>
/// Displays the list of queued converter jobs and their current status
/// inside the Inspector tab.
/// </summary>
internal class BuildView
{
    private const int SOURCE_COLUMN = 1;
    private const int TARGET_COLUMN = 5;
    private const int FULL_SOURCE_PATH_COLUMN = 6;
    private const int FULL_TARGET_PATH_COLUMN = 7;

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
            DataCell = new ImageTextCell(4, TARGET_COLUMN),
        });

        // Hidden columns for storing full paths for double-click actions.
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Full Source Path",
            AutoSize = true,
            DataCell = new TextBoxCell(FULL_SOURCE_PATH_COLUMN),
            Visible = false,
        });
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Full Target Path",
            AutoSize = true,
            DataCell = new TextBoxCell(FULL_TARGET_PATH_COLUMN),
            Visible = false,
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

            GetSourceDisplay(queuedItem.Item.Source, out string sourceDisplayName, out string fullSourcePath);
            var conversion = $"{queuedItem.Item.SourceType} → {queuedItem.Item.TargetType}";
            var status = GetStatusText(queuedItem);
            GetTargetDisplay(queuedItem, out string displayTargetPath, out string fullTargetPath);

            _rootItem.Children.Add(new TreeGridItem
            {
                Values = [Icons.FileIcon, sourceDisplayName, conversion, status, Icons.FileIcon, displayTargetPath, fullSourcePath, fullTargetPath],
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

    private static void GetSourceDisplay(object source, out string displayName, out string fullFilePath)
    {
        if (source is Ktx2PipelineSource ktx2Source)
        {
            fullFilePath = Path.GetFullPath(ktx2Source.FilePath);
            displayName = Path.GetFileName(fullFilePath);
            return;
        }
        if (source is GenericPipelineSource genericSource)
        {
            fullFilePath = Path.GetFullPath(genericSource.FilePath);
            displayName = Path.GetFileName(fullFilePath);
            return;
        }

        if (source is string str)
        {
            displayName = str;
            fullFilePath = str;
            return;
        }

        displayName = "Unknown";
        fullFilePath = displayName;
    }

    private static void GetTargetDisplay(QueuedPipelineItem pipelineQueue, out string displayName, out string fullFilePath)
    {
        var result = pipelineQueue.Result;
        displayName = "";
        fullFilePath = "";

        if (result?.Success != true)
        {
            return;
        }

        if (pipelineQueue.Item.Options is Ktx2PipelineOptions ktx2Options)
        {
            fullFilePath = Path.GetFullPath(ktx2Options.OutputPath);
            displayName = Path.GetFileName(fullFilePath);
            return;
        }

        if (pipelineQueue.Item.Options is GenericPipelineOptions genericOptions)
        {
            fullFilePath = Path.GetFullPath(genericOptions.OutputPath);
            displayName = Path.GetFileName(fullFilePath);
            return;
        }

        // Fallback for unknown pipeline types
        displayName = "Unknown";
        fullFilePath = "";
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
        var fullSourcePath = treeGridItem.Values[FULL_SOURCE_PATH_COLUMN];
        var fullTargetPath = treeGridItem.Values[FULL_TARGET_PATH_COLUMN];

        // If it's target double-click, load both images in the viewer.
        if (!String.IsNullOrEmpty(fullTargetPath.ToString()))
        {
            var imageViewer = new ImageViewerWindow(_context);
            var sourceFilePath = fullSourcePath.ToString();
            var destFilePath = Path.Combine(AppContext.BaseDirectory, "Build", fullTargetPath.ToString());
            imageViewer.View([destFilePath, sourceFilePath]);
        }
    }
}