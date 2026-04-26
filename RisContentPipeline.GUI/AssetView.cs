using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.TreeGridItems;

namespace RisContentPipeline.GUI;

/// <summary>
/// The AssetView class is responsible for displaying the list of assets in a tree view format.
/// </summary>
internal class AssetView
{
    private readonly Context _context;

    /// <summary>
    /// The tree view that displays the list of assets. 
    /// This will be populated with the contents of the "Assets" folder and allow users to select assets for processing.
    /// </summary>
    internal TreeGridView AssetTreeView { get; }

    /// <summary>
    /// The constructor initializes the asset view, setting up the tree view and loading sample assets for demonstration purposes.
    /// </summary>
    public AssetView(Context context)
    {
        // Create tree view for assets
        AssetTreeView = new TreeGridView
        {
            Size = new Size(250, -1)
        };

        // Add column to tree view
        AssetTreeView.Columns.Add(new GridColumn
        {
            HeaderText = "Assets",
            DataCell = new ImageTextCell(0, 1)
        });

        // Set up tree view event handlers
        AssetTreeView.SelectionChanged += OnAssetSelectionChanged;
        _context = context;

        //// Initialize with sample data
        //LoadSampleAssets();
    }

    public void Refresh()
    {
        // Create a placeholder icon for the root folder
        var rootItem = new ImageTreeGridItem("Assets", null, Icons.FolderIcon);

        foreach (var fileOrFolder in _context.FilesOrFolders)
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
            else if(fileOrFolder.IsJson || fileOrFolder.IsXml)
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
        AssetTreeView.DataStore = rootItem;

        // Expand the root
        rootItem.Expanded = true;
    }

    //private void LoadSampleAssets()
    //{
    //    // Create a placeholder icon for the root folder
    //    var rootItem = new ImageTreeGridItem("Assets", Icons.FolderIcon, null);

    //    // Add some sample image files (you can replace this with actual file scanning)
    //    var sampleImages = new[]
    //    {
    //        "sample1.png",
    //        "sample2.jpg",
    //        "sample3.bmp",
    //        "textures/wood.png",
    //        "textures/metal.jpg",
    //        "ui/icons/button.png"
    //    };

    //    foreach (var imagePath in sampleImages)
    //    {
    //        // Create a placeholder thumbnail
    //        var thumbnail = Icons.ImageIcon;
    //        var item = new ImageTreeGridItem(
    //            Path.GetFileName(imagePath),
    //            thumbnail,
    //            imagePath
    //        );
    //        rootItem.Children.Add(item);
    //    }

    //    // Set the data store
    //    AssetTreeView.DataStore = rootItem;

    //    // Expand the root
    //    rootItem.Expanded = true;
    //}



    private void OnAssetSelectionChanged(object? sender, EventArgs e)
    {
        if (AssetTreeView.SelectedItem is ImageTreeGridItem selectedItem && selectedItem.FileOrFolder != null)
        {
            // For now, just handle the selection
            // In a real implementation, you would notify the main form or trigger an event
            // For example: AssetSelected?.Invoke(this, new AssetSelectedEventArgs(selectedItem.FilePath));
        }
    }

    ///// <summary>
    ///// Loads assets from a specified directory path.
    ///// </summary>
    ///// <param name="directoryPath">The directory path to scan for assets.</param>
    //public void LoadAssetsFromDirectory(string directoryPath)
    //{
    //    if (!Directory.Exists(directoryPath))
    //    {
    //        return;
    //    }

    //    var rootItem = new ImageTreeGridItem(Path.GetFileName(directoryPath), Icons.FolderIcon, directoryPath);

    //    // Get all image files from the directory
    //    var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga", ".webp" };
    //    var imageFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
    //        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

    //    foreach (var imagePath in imageFiles)
    //    {
    //        var thumbnail = Icons.ImageIcon;
    //        var item = new ImageTreeGridItem(
    //            Path.GetFileName(imagePath),
    //            thumbnail,
    //            imagePath
    //        );
    //        rootItem.Children.Add(item);
    //    }

    //    AssetTreeView.DataStore = rootItem;
    //    rootItem.Expanded = true;
    //}
}
