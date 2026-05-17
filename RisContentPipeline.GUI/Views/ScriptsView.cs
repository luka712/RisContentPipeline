using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.TreeGridItems;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// The ScriptsView class is responsible for displaying the list of scripts to be executed on build.
/// </summary>
internal class ScriptsView
{
    private readonly Context _context;
    private readonly TreeGridView _treeView;

    /// <summary>
    /// The titled panel that hosts the script tree view.
    /// </summary>
    internal Control Content { get; }

    // Store the last clicked cell for context menu actions
    private int? _lastClickedRow = null;

    private void HandleRemoveScriptItem()
    {
        if (_lastClickedRow.HasValue)
        {
            var item = _treeView.DataStore[_lastClickedRow.Value];
            if (item is TreeGridItem treeGridItem)
            {
                var scriptName = treeGridItem.Values[1].ToString();
                var scriptToRemove = _context.BuildScripts.FirstOrDefault(s => s.Name == scriptName);
                if (scriptToRemove != null)
                {
                    _context.RemoveBuildScript(scriptToRemove);
                    Refresh();
                }
            }

        }
    }

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="context">The <see cref="Context"/>.</param>
    public ScriptsView(Context context)
    {
        _context = context;

        _treeView = new()
        {
            ShowHeader = false,
            AllowMultipleSelection = false,
            Border = BorderType.None,
        };
        _treeView.CellClick += (sender, args) => _lastClickedRow = args.Row;

        _treeView.ContextMenu = new ContextMenu
        {
            Items =
            {
                new ButtonMenuItem { Text = "Remove Script", Command = new Command((_, __) => HandleRemoveScriptItem()) },
            }
        };

        // Add a column to the tree view
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Scripts",
            AutoSize = true,
            DataCell = new ImageTextCell(0, 1)
        });

        // Set up tree view event handlers
        _treeView.SelectionChanged += OnAssetSelectionChanged;

        Content = new GroupBox
        {
            Text = "Build Scripts",
            Font = SystemFonts.Bold(),
            Padding = new Padding(Theme.PADDING),
            Content = _treeView,
        };

        _context.OnBuildScriptAdded += (_, _) => Refresh();
    }

    /// <summary>
    /// Refreshes the script tree view.
    /// </summary>
    internal void Refresh()
    {
        // Create a placeholder icon for the root folder
        var rootItem = new ImageTreeGridItem("Scripts", null, Icons.FolderIcon);

        foreach (var script in _context.BuildScripts)
        {
            var item = new TreeGridItem
            {
                Values = [Icons.PythonIcon, script.Name],
            };
            rootItem.Children.Add(item);
        }

        // Set the data store
        _treeView.DataStore = rootItem;

        // Expand the root
        rootItem.Expanded = true;
    }

    private void OnAssetSelectionChanged(object? sender, EventArgs e)
    {
        if (_treeView.SelectedItem is ImageTreeGridItem selectedItem && selectedItem.FileOrFolder != null)
        {
            // Selection currently has no further side effects.
        }
    }
}
