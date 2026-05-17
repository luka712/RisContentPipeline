using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.TreeGridItems;
using RisContentPipeline.GUI.Windows;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// The AssetView class is responsible for displaying the list of assets in a tree view format.
/// </summary>
internal class AssetView
{
    private readonly Context _context;
    private readonly TreeGridView _treeView;

    private ImageViewerWindow? _imageViewerWindow;

    /// <summary>
    /// The titled panel that hosts the asset tree view.
    /// </summary>
    internal Control Content { get; }

    /// <summary>
    /// The constructor initializes the asset view, setting up the tree view and loading sample assets for demonstration purposes.
    /// </summary>
    public AssetView(Context context)
    {
        _context = context;

        _treeView = new TreeGridView
        {
            ShowHeader = false,
            AllowMultipleSelection = false,
            Border = BorderType.None,
        };


        // Add a column to the tree view
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Assets",
            AutoSize = true,
            DataCell = new ImageTextCell(0, 1)
        });

        // Set up tree view event handlers
        _treeView.SelectionChanged += OnAssetSelectionChanged;
        _treeView.CellDoubleClick += OnAssetDoubleClick;

        Content = new GroupBox
        {
            Text = "Assets",
            Font = SystemFonts.Bold(),
            Padding = new Padding(Theme.PADDING),
            Content = _treeView,
        };
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
                    fileOrFolder.PathOrFileName ?? string.Empty,
                    fileOrFolder,
                    thumbnail
                );
                rootItem.Children.Add(item);
            }
            else if (fileOrFolder.IsJson || fileOrFolder.IsXml)
            {
                var item = new ImageTreeGridItem(
                    fileOrFolder.PathOrFileName ?? string.Empty,
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

    private void OnAssetDoubleClick(object? sender, EventArgs e)
    {
        // If it's image item, we want to open the image viewer.
        if (_treeView.SelectedItem is ImageTreeGridItem { FileOrFolder.IsImage: true } selectedItem)
        {
            _imageViewerWindow = new (_context);
            _imageViewerWindow.Show();
            _imageViewerWindow.FilePath = selectedItem.FileOrFolder.AbsolutePathOrFileName;
        }
    }
}