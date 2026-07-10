using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RisContentPipeline.GUI.Services;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// A window that manages the local web server for viewing KTX2 and PNG images.
/// Opens the viewer in the system's default browser.
/// </summary>
public partial class ImageViewerWindow : Window
{
    private readonly LocalWebServer? _webServer;
    private readonly string _serverUrl;
    private string? _currentFilePath;

    public ImageViewerWindow()
    {
        InitializeComponent();
        _serverUrl = "http://localhost:5050";
    }

    /// <summary>
    /// Creates an ImageViewerWindow with a local web server.
    /// </summary>
    /// <param name="viewerRootPath">The root path where the viewer HTML/JS files are located.</param>
    /// <param name="serverPort">The port for the local web server.</param>
    public ImageViewerWindow(string viewerRootPath, int serverPort) : this()
    {
        _serverUrl = $"http://localhost:{serverPort}";
        UrlText.Text = _serverUrl;

        // Start local web server if viewer path exists
        if (Directory.Exists(viewerRootPath))
        {
            _webServer = new LocalWebServer(viewerRootPath, serverPort);
            _webServer.Start();
        }
    }

    /// <summary>
    /// Opens the viewer and loads the specified image files.
    /// </summary>
    /// <param name="filePaths">The paths to the image files.</param>
    public void View(params string[] filePaths)
    {
        if (filePaths.Length > 0)
        {
            _currentFilePath = filePaths[0];
            FilePathText.Text = _currentFilePath;
        }

        Show();
        OpenInBrowser();
    }

    private void OpenInBrowser()
    {
        try
        {
            // Build URL with file parameter if we have a file
            var url = _serverUrl;
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                var fileName = Path.GetFileName(_currentFilePath);
                var base64 = Convert.ToBase64String(File.ReadAllBytes(_currentFilePath));
                // For now, just open the viewer - the file will need to be loaded via the UI
                // In a more complete implementation, we could pass the file via query params or local storage
            }

            // Open in default browser
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            // Log error but don't crash
            System.Diagnostics.Debug.WriteLine($"Failed to open browser: {ex.Message}");
        }
    }

    private void OnOpenBrowserClick(object? sender, RoutedEventArgs e)
    {
        OpenInBrowser();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _webServer?.Dispose();
    }
}
