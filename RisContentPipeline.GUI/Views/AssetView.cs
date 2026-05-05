using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.TreeGridItems;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// The AssetView class is responsible for displaying the list of assets in a tree view format.
/// </summary>
internal class AssetView
{
    private readonly Context _context;
    private readonly TreeGridView _treeView;

    /// <summary>
    /// The tree view that displays the list of assets. 
    /// This will be populated with the contents of the "Assets" folder and allow users to select assets for processing.
    /// </summary>
    internal DynamicLayout Content { get; }

    /// <summary>
    /// The constructor initializes the asset view, setting up the tree view and loading sample assets for demonstration purposes.
    /// </summary>
    public AssetView(Context context)
    {
        // Create a tree view for assets
        Content = new DynamicLayout()
        {
            Width = Theme.SIDE_PANELS_WIDTH - Theme.PADDING * 2,
            Height = -1, 
        };
        Content.BeginVertical();
        _treeView = new TreeGridView();
        Content.AddRow(_treeView);
        Content.EndVertical();

        // Add a column to the tree view
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Assets",
            DataCell = new ImageTextCell(0, 1)
        });

        // Set up tree view event handlers
        _treeView.SelectionChanged += OnAssetSelectionChanged;
        _context = context;

        //// Initialize with sample data
        //LoadSampleAssets();
    }

    public void Refresh()
    {
        // Create a placeholder icon for the root folder
        var rootItem = new ImageTreeGridItem("Assets", null, Icons.FolderIcon);

        var filesOrFolders = _context.FilesOrFolders.OrderBy(f => f.PathOrFileName).ToList();
        foreach (var fileOrFolder in filesOrFolders)
        {
            // Create a placeholder thumbnail
            if (fileOrFolder.Image != null)
            {
                var thumbnail = Icons.ImageIcon;
                var item = new ImageTreeGridItem(
                    fileOrFolder.PathOrFileName,
                    fileOrFolder,
                    thumbnail
                );
                rootItem.Children.Add(item);
            }
            else if (fileOrFolder.IsJson || fileOrFolder.IsXml)
            {
                var item = new ImageTreeGridItem(
                    fileOrFolder.PathOrFileName,
                    fileOrFolder,
                    Icons.FileIcon
                );
                rootItem.Children.Add(item);
            }
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
            _context.OnItemSelected?.Invoke(selectedItem.FileOrFolder);
        }
    }
}