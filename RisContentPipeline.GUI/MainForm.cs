using Eto.Forms;
using Eto.Drawing;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Views;


namespace RisContentPipeline.GUI;

public sealed class MainForm : Form
{
    private readonly Context _context = new ();
    private readonly BuildLogger _messanger = new BuildLogger();

    private ActionsBar _actionsBar;
    private AssetView _assetView;
    private BuildView _buildView;
    private Label _statusLabel;
    private TextBox _inputBox;
    private TextBox _outputBox;

    public MainForm()
    {
        Icons.Load();

        Title = "RisContentPipeline";
        ClientSize = new Eto.Drawing.Size(1000, 600);
        Resizable = true;

        // Create menu
        Menu = CreateMenu();

        // Create actions bar
        _actionsBar = new ActionsBar(this, _context);
        _actionsBar.FileAdded += OnFileAdded;
        _actionsBar.FolderAdded += OnFolderAdded;

        _assetView = new AssetView(_context);
        _buildView = new BuildView(_context);

        // Create the right panel
        var rightPanel = CreateRightPanel();

        // Create main content layout
        var contentLayout = new TableLayout
        {
            Spacing = new Eto.Drawing.Size(8, 8),
            Padding = new Padding(8),
            Rows =
            {
                new TableRow(
                    new TableCell(_assetView.AssetTreeView, false),
                    new TableCell(_buildView.BuildOutputTreeView, false)
                )
            }
        };

        // Create root layout with toolbar and content
        var rootLayout = new DynamicLayout();
        rootLayout.Add(_actionsBar.Panel);
        rootLayout.Add(contentLayout, yscale: true);

        Content = rootLayout;
    }

    private MenuBar CreateMenu()
    {
        var fileMenu = new ButtonMenuItem { Text = "&File" };
        fileMenu.Items.Add(new Command { MenuText = "E&xit", Shortcut = Application.Instance.CommonModifier | Keys.Q });

        var editMenu = new ButtonMenuItem { Text = "&Edit" };
        
        var helpMenu = new ButtonMenuItem { Text = "&Help" };
        helpMenu.Items.Add(new Command { MenuText = "&About..." });

        return new MenuBar
        {
            Items = { fileMenu, editMenu },
            HelpItems = { helpMenu }
        };
    }

    private void OnFileAdded(object? sender, string filePath)
    {
        UpdateStatus($"Added file: {Path.GetFileName(filePath)}");
        // TODO: Add file to asset view
        _assetView.Refresh();
    }

    private void OnFolderAdded(object? sender, string folderPath)
    {
        UpdateStatus($"Added folder: {Path.GetFileName(folderPath)}");
       // _assetView.LoadAssetsFromDirectory(folderPath);
    }

    private Panel CreateRightPanel()
    {
        var rightPanel = new Panel();
        
        _inputBox = new TextBox { PlaceholderText = "Input image file (.png/.jpg/.bmp/.gif/.tga...)" };
        _outputBox = new TextBox { PlaceholderText = "Output .ktx2 file" };
        _statusLabel = new Label();

        var browseInputButton = new Button { Text = "Browse input" };
        browseInputButton.Click += (_, _) =>
        {
            using var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileFilter("Images", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga", ".webp"));
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                _inputBox.Text = dialog.FileName;
                UpdateStatus($"Selected input: {Path.GetFileName(dialog.FileName)}");
            }
        };

        var browseOutputButton = new Button { Text = "Browse output" };
        browseOutputButton.Click += (_, _) =>
        {
            using var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileFilter("KTX2", ".ktx2"));
            dialog.FileName = "output.ktx2";
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                _outputBox.Text = dialog.FileName;
                UpdateStatus($"Selected output: {Path.GetFileName(dialog.FileName)}");
            }
        };

        var convertButton = new Button { Text = "Convert to KTX2" };
        convertButton.Click += (_, _) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_inputBox.Text) || string.IsNullOrWhiteSpace(_outputBox.Text))
                {
                    UpdateStatus("Please specify both input and output files.");
                    return;
                }

                //using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(_inputBox.Text);
                //var pipeline = new Ktx2Converter();
                //pipeline.Convert(image, _outputBox.Text, null);
                
                UpdateStatus("Conversion complete.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Conversion failed: {ex.Message}");
            }
        };

        var clearButton = new Button { Text = "Clear" };
        clearButton.Click += (_, _) =>
        {
            _inputBox.Text = string.Empty;
            _outputBox.Text = string.Empty;
            UpdateStatus("Cleared input and output fields.");
        };

        rightPanel.Content = new TableLayout
        {
            Padding = 12,
            Spacing = new Eto.Drawing.Size(8, 8),
            Rows =
            {
                new TableRow(_inputBox, browseInputButton),
                new TableRow(_outputBox, browseOutputButton),
                new TableRow(convertButton, clearButton),
                new TableRow(_statusLabel)
            }
        };

        return rightPanel;
    }

    private void UpdateStatus(string message)
    {
        _statusLabel.Text = message;
    }
}