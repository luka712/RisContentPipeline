using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Modals;


namespace RisContentPipeline.GUI.Views;

/// <summary>
/// Represents the actions bar that contains common operations like adding files and folders.
/// </summary>
internal class ActionsBarView
{
    private static readonly string[] _supportedFileExtensions = [".png", ".ktx2", ".json", ".xml"];
    private readonly Context _context;


    /// <summary>
    /// Gets the panel control containing the action buttons.
    /// </summary>
    public Panel Panel { get; }

    /// <summary>
    /// Event raised when a file should be added.
    /// </summary>
    public event EventHandler<string>? FileAdded;

    /// <summary>
    /// Event raised when a folder should be added.
    /// </summary>
    public event EventHandler<string>? FolderAdded;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionsBarView"/> class.
    /// </summary>
    /// <param name="parentWindow">The parent window for dialogs.</param>
    public ActionsBarView(Window parentWindow, Context context)
    {
        _context = context;

        // Create buttons
        var addFileButton = new Button
        {
            Text = "Add File",
            ToolTip = "Add an existing asset file to the project",
            Width = 100
        };
        addFileButton.Click += (sender, e) => ShowAddFileDialog(parentWindow);

        var addFolderButton = new Button
        {
            Text = "Add Folder",
            ToolTip = "Add an existing folder and its contents to the project",
            Width = 100
        };
        addFolderButton.Click += (sender, e) => ShowAddFolderDialog(parentWindow);

        var buildButton = new Button
        {
            Text = "Build",
            ToolTip = "Build all assets in the project",
            Width = 80
        };
        buildButton.Click += (sender, e) =>
        {
            _context.Build();
        };

        var rebuildButton = new Button
        {
            Text = "Rebuild",
            ToolTip = "Rebuild all assets in the project",
            Width = 80
        };
        rebuildButton.Click += (sender, e) =>
        {
            // TODO: Implement rebuild functionality
            MessageBox.Show(parentWindow, "Rebuild functionality coming soon!", MessageBoxType.Information);
        };

        var cleanButton = new Button
        {
            Text = "Clean",
            ToolTip = "Clean all built assets",
            Width = 80
        };
        cleanButton.Click += (sender, e) =>
        {
            // TODO: Implement clean functionality
            MessageBox.Show(parentWindow, "Clean functionality coming soon!", MessageBoxType.Information);
        };

        var addBuildScript = new Button
        {
            Text = "Add Build Script",
            ToolTip = "Add a custom Python script to the build process",
            Width = 120
        };
        addBuildScript.Click += (sender, e) =>
        {
            using var dialog = new SelectScriptsModal(_context);
            var selectedScripts = dialog.ShowModal(parentWindow);
            if (selectedScripts != null && selectedScripts.Any())
            {
                foreach(var script in selectedScripts)
                {
                    _context.AddBuildScript(script);
                }
            }
        };

        // Create horizontal layout for buttons
        var layout = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5,
            Padding = new Padding(5),
            Items =
            {
                addFileButton,
                addFolderButton,
                new StackLayoutItem {  }, // Spacer
                buildButton,
                rebuildButton,
                cleanButton,
                addBuildScript
            }
        };


        // Wrap in panel with background
        Panel = new Panel
        {
            Content = layout,
            BackgroundColor = SystemColors.ControlBackground,
            Padding = new Padding(2)
        };
    }

    /// <summary>
    /// Shows the add file dialog and raises the FileAdded event if a file is selected.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    private void ShowAddFileDialog(Window parent)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Add Existing File",
            MultiSelect = true
        };

        dialog.Filters.Add(new FileFilter("Files", _supportedFileExtensions));
        dialog.Filters.Add(new FileFilter("All Files", ".*"));

        if (dialog.ShowDialog(parent) == DialogResult.Ok)
        {
            foreach (var file in dialog.Filenames)
            {
                OnFileAdded(file);
            }
        }
    }

    /// <summary>
    /// Shows the select folder dialog and raises the FolderAdded event if a folder is selected.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    private void ShowAddFolderDialog(Window parent)
    {
        using var dialog = new SelectFolderDialog
        {
            Title = "Add Existing Folder"
        };

        if (dialog.ShowDialog(parent) == DialogResult.Ok)
        {
            OnFolderAdded(dialog.Directory);
        }
    }

    /// <summary>
    /// Raises the FileAdded event.
    /// </summary>
    /// <param name="filePath">The file path that was added.</param>
    protected virtual void OnFileAdded(string filePath)
    {
        _context.AddFileAsync(filePath).ContinueWith(task =>
        {
            FileAdded?.Invoke(this, filePath);
        });
    }

    /// <summary>
    /// Raises the FolderAdded event.
    /// </summary>
    /// <param name="folderPath">The folder path that was added.</param>
    protected virtual void OnFolderAdded(string folderPath)
    {
        FolderAdded?.Invoke(this, folderPath);
    }
}