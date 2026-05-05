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
    /// The tree view that displays the list of scripts. 
    /// </summary>
    internal DynamicLayout Content { get; }

    /// <summary>
    /// The constructor.
    /// <param name="context">The <see cref="Context"/>.</param>"
    /// </summary>
    public ScriptsView(Context context)
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
        Content.AddAutoSized(null);
        Content.EndVertical();
        
        // Add a column to the tree view
        _treeView.Columns.Add(new GridColumn
        {
            HeaderText = "Scripts",
            DataCell = new ImageTextCell(0, 1)
        });

        // Set up tree view event handlers
        _treeView.SelectionChanged += OnAssetSelectionChanged;
        _context = context;

        _context.OnBuildScriptAdded += (_, script) =>
        {
            Refresh();
        };

        Refresh();
    }

    public void Refresh()
    {
        // Create a placeholder icon for the root folder
        var rootItem = new ImageTreeGridItem("Scripts", null, Icons.FolderIcon);

        foreach (var script in _context.BuildScripts)
        {
            //var thumbnail = Icons.ImageIcon;
            var item = new TreeGridItem()
            {
                Values = [Icons.PythonIcon, script.Name]
            };
            rootItem.Children.Add(item);
        }

        // Set the data store
        _treeView.DataStore = rootItem;

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
        if (_treeView.SelectedItem is ImageTreeGridItem selectedItem && selectedItem.FileOrFolder != null)
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
