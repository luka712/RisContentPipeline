using Eto.Forms;
using Eto.Drawing;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.Views;
using System.ComponentModel;
using RisContentPipeline.GUI.Controls;
using RisContentPipeline.GUI.Views.Inspector;
using RisContentPipeline.GUI.Windows;

namespace RisContentPipeline.GUI;

/// <summary>
/// The main application window.
/// Hosts the action bar at the top, three resizable columns of views in the middle
/// (assets/scripts | inspector + build output | settings), and a status bar at the bottom.
/// </summary>
public sealed class MainForm : Form
{
    private readonly Context _context = new();
    private LocalWebServer _server;

    
    private readonly ActionsBarView _actionsBar;
    private readonly AssetView _assetView;
    private readonly ScriptsView _scriptsView;
    private readonly MessageView _messageView;
    private readonly SettingsView _settingsView;
    private readonly InspectorView _inspectorView;
    private readonly Label _statusLabel = new() { Text = "Ready" };

    public MainForm()
    {
        Icons.Load();

        Title = "RisContentPipeline";
        ClientSize = new Size(Theme.CLIENT_WIDTH, Theme.CLIENT_HEIGHT);
        MinimumSize = new Size(900, 540);
        Resizable = true;
        _context.LoadSession();
        
        

        // Create a menu
        Menu = CreateMenu();

        // Create an actions bar
        _actionsBar = new ActionsBarView(this, _context);
        _actionsBar.FileAdded += OnFileAdded;
        _actionsBar.FolderAdded += OnFolderAdded;

        _assetView = new AssetView(_context);
        _scriptsView = new ScriptsView(_context);
        _messageView = new MessageView(_context);
        _settingsView = new SettingsView(_context);
        _inspectorView = new InspectorView(this, _context);

        WireBuildLoggerToStatus();

        // ---- Left column: assets stacked above scripts (resizable vertically) ----
        var leftSplitter = new Splitter
        {
            Orientation = Orientation.Vertical,
            Panel1 = _assetView.Content,
            Panel2 = _scriptsView.Content,
            Position = (Theme.CLIENT_HEIGHT - Theme.BUILD_OUTPUT_HEIGHT) / 2,
            FixedPanel = SplitterFixedPanel.None,
        };

        // ---- Center column: inspector stacked above build output (resizable vertically) ----
        var centerSplitter = new Splitter
        {
            Orientation = Orientation.Vertical,
            Panel1 = _inspectorView.Content,
            Panel2 = _messageView.Content,
            Position = Theme.CLIENT_HEIGHT - Theme.BUILD_OUTPUT_HEIGHT - 80,
            FixedPanel = SplitterFixedPanel.Panel2,
        };

        // ---- Right column: settings panel ----
        var rightPanel = _settingsView.Content;

        // ---- Center+right horizontal splitter ----
        var centerRightSplitter = new Splitter
        {
            Orientation = Orientation.Horizontal,
            Panel1 = centerSplitter,
            Panel2 = rightPanel,
            Position = Theme.CLIENT_WIDTH - Theme.RIGHT_SIDE_PANELS_MIN_WIDTH,
            FixedPanel = SplitterFixedPanel.Panel2,
            Panel2MinimumSize = Theme.RIGHT_SIDE_PANELS_MIN_WIDTH,
        };

        // ---- Top-level horizontal splitter (left | (center | right)) ----
        var mainSplitter = new Splitter
        {
            Orientation = Orientation.Horizontal,
            Panel1 = leftSplitter,
            Panel2 = centerRightSplitter,
            Position = Theme.LEFT_SIDE_PANELS_MIN_WIDTH,
            FixedPanel = SplitterFixedPanel.Panel1,
            Panel1MinimumSize = Theme.LEFT_SIDE_PANELS_MIN_WIDTH,
        };

        // ---- Status bar ----
        var statusBar = new Panel
        {
            BackgroundColor = SystemColors.ControlBackground,
            Padding = new Padding(8, 4),
            Content = _statusLabel,
        };

        // ---- Root layout: actions bar / main / status bar ----
        var rootLayout = new DynamicLayout();
        rootLayout.Add(_actionsBar.Panel);
        rootLayout.Add(mainSplitter, yscale: true);
        rootLayout.Add(statusBar);

        Content = rootLayout;
        
        StartServer();

    }

    private void StartServer()
    {
        string contentFolder = Path.Combine(AppContext.BaseDirectory, "_image_viewer");
        _server = new LocalWebServer(_context.MessageLogger, contentFolder, port: _context.LocalServerPort);
        _ = _server.StartAsync();
    }

    /// <summary>
    /// Mirrors interesting <see cref="MessageLogger"/> events into the status bar so the
    /// user always has a quick read-out of the last action.
    /// </summary>
    private void WireBuildLoggerToStatus()
    {
        var logger = _context.MessageLogger;
        logger.OnInfoLog += msg => UpdateStatus(msg);
        logger.OnSuccessLog += msg => UpdateStatus(msg);
        logger.OnErrorLog += msg => UpdateStatus($"Error: {msg}");
    }

    private MenuBar CreateMenu()
    {
        var fileMenu = new ButtonMenuItem { Text = "&File" };
        fileMenu.Items.Add(new Command { MenuText = "E&xit", Shortcut = Application.Instance.CommonModifier | Keys.Q });

        // EDIT
        var editMenu = new ButtonMenuItem { Text = "&Edit" };

        // - Preferences
        var preferencesCommand = new Command
        {
            MenuText = "&Preferences...",
            Shortcut = Application.Instance.CommonModifier | Keys.Comma,
        };
        preferencesCommand.Executed += (sender, e) =>
        {
            var preferencesWindow = new Windows.PreferencesWindow(_context);
            preferencesWindow.ShowModal(this);
        };
        editMenu.Items.Add(preferencesCommand);

        // WINDOWS
        var windowsMenu = new ButtonMenuItem { Text = "&Windows" };

        // - Image Viewer
        var imageViewerCommand = new Command
        {
            MenuText = "&Image Viewer",
            Shortcut = Application.Instance.CommonModifier | Keys.I,
        };
        imageViewerCommand.Executed += (sender, e) =>
        {
            var imageViewerWindow = new ImageViewerWindow(_context);
            imageViewerWindow.Show();
        };
        windowsMenu.Items.Add(imageViewerCommand);

        var helpMenu = new ButtonMenuItem { Text = "&Help" };
        helpMenu.Items.Add(new Command { MenuText = "&About..." });

        return new MenuBar
        {
            Items = { fileMenu, editMenu, windowsMenu },
            HelpItems = { helpMenu }
        };
    }

    private void OnFileAdded(object? sender, string filePath)
    {
        _context.AsyncInvoke(() =>
        {
            UpdateStatus($"Added file: {Path.GetFileName(filePath)}");
            _assetView.Refresh();
        });
    }

    private void OnFolderAdded(object? sender, string folderPath)
    {
        UpdateStatus($"Added folder: {Path.GetFileName(folderPath)}");
        // _assetView.LoadAssetsFromDirectory(folderPath);
    }

    private void UpdateStatus(string message)
    {
        if (_statusLabel != null)
        {
            _statusLabel.Text = message;
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        _context.SaveSession();
    }
}
