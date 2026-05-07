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
    /// <param name="context">The application <see cref="Context"/>.</param>
    public ActionsBarView(Window parentWindow, Context context)
    {
        _context = context;

        // Create buttons
        var addFileButton = CreateButton("Add File", "Add an existing asset file to the project", 100);
        addFileButton.Click += (sender, e) => ShowAddFileDialog(parentWindow);

        var addFolderButton = CreateButton("Add Folder", "Add an existing folder and its contents to the project", 110);
        addFolderButton.Click += (sender, e) => ShowAddFolderDialog(parentWindow);

        var buildButton = CreateButton("Build", "Build all assets in the project", 80);
        buildButton.Click += (sender, e) => _context.Build();

        var rebuildButton = CreateButton("Rebuild", "Rebuild all assets in the project", 90);
        rebuildButton.Click += (sender, e) => _context.Rebuild();

        var cleanButton = CreateButton("Clean", "Clean all built assets", 80);
        cleanButton.Click += (sender, e) => _context.Clean();

        var addBuildScript = CreateButton("Add Build Script", "Add a custom Python script to the build process", 140);
        addBuildScript.Click += (sender, e) =>
        {
            using var dialog = new SelectScriptsModal(_context);
            var selectedScripts = dialog.ShowModal(parentWindow);
            if (selectedScripts != null && selectedScripts.Any())
            {
                foreach (var script in selectedScripts)
                {
                    _context.AddBuildScript(script);
                }
            }
        };

        // Create horizontal layout for buttons.
        // The empty StackLayoutItem with Expand=true acts as a flexible spacer
        // that pushes the build/script buttons to the right side of the bar.
        var layout = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            Spacing = Theme.CONTROL_SPACING,
            Padding = new Padding(Theme.PADDING),
            VerticalContentAlignment = VerticalAlignment.Center,
            Items =
            {
                addFileButton,
                addFolderButton,
                CreateSeparator(),
                addBuildScript,
                new StackLayoutItem(null, expand: true), // flexible spacer
                buildButton,
                rebuildButton,
                cleanButton,
            }
        };

        Panel = new Panel
        {
            Content = layout,
            BackgroundColor = SystemColors.ControlBackground,
        };
    }

    private static Button CreateButton(string text, string tooltip, int width) => new()
    {
        Text = text,
        ToolTip = tooltip,
        Width = width,
        Height = Theme.BUTTON_HEIGHT,
    };

    private static Control CreateSeparator() => new Panel
    {
        Width = 1,
        Height = Theme.BUTTON_HEIGHT - 4,
        BackgroundColor = SystemColors.ControlBackground.ToArgb() == 0 ? Colors.Gray : Colors.DarkGray,
    };

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
