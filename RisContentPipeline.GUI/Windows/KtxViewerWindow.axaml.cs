using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.ViewModels;
using RisContentPipeline.Ktx2;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Windows;

public partial class KtxViewerWindow : SukiWindow
{
   
    private readonly LocalWebServer _server;
    private readonly WindowsService _windowsService;
    private readonly NativeWebView _webView;

    /// <summary>
    /// The constructor.
    /// </summary>
    public KtxViewerWindow()
    {
        _server = App.Services.GetService<LocalWebServer>()!;
        _windowsService = App.Services.GetService<WindowsService>()!;
        InitializeComponent();
        _webView = this.FindControl<NativeWebView>("PartNativeWebView")!;
    }
    
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _webView.Source = new Uri($"http://localhost:{_server.Port}");
    }

    private void PartNativeWebView_OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        // BUG: Avalonia Bug most likely. WebView doesn't update its size until the next frame.
        Dispatcher.UIThread.Post(() =>
        {
            _webView.InvalidateArrange();
            _webView.InvalidateVisual();
        }, DispatcherPriority.Render);
        // BUG: END

        var viewModel = DataContext as QueuedPipelineItemViewModel;

        var conversionItem = viewModel.Item.Item;
        if (conversionItem.Options is Ktx2PipelineOptions ktxOptions)
        {
            _ = LoadImageAsync(ktxOptions.OutputPath);
        }
    }

    /// <summary>
    /// Loads the image into the browser viewer.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    private async Task LoadImageAsync(string filePath)
    {
        var isKtx2 = filePath.EndsWith(".ktx2");

        // js function from viewer to call.
        var jsFunction = isKtx2 ? "loadKtx2TextureFromBase64" : "loadPngTextureFromBase64";
            
        var fileName = Path.GetFileName(filePath);

        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File '{filePath}' not found.", filePath);
        }
        
        byte[] data = await File.ReadAllBytesAsync(filePath);
        string base64 = Convert.ToBase64String(data);
        
        Dispatcher.UIThread.Post(() =>
        {;
            _ = _webView.InvokeScript($"window.{jsFunction}('{base64}');");
        });
    }
}