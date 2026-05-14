using Photino.NET;

namespace RisContentPipeline.GUI.Controls;

public class PhotinoWebView
{
    private LocalWebServer? _server;
    private PhotinoWindow? _window;
    
    public PhotinoWebView()
    {
        string contentFolder = Path.Combine(AppContext.BaseDirectory, "Ktx2BrowserViewerContent");

        _server = new LocalWebServer(contentFolder, port: 8080);
        _ = _server.StartAsync();
        
            _window = new PhotinoWindow()
                .SetTitle("Ktx2 Viewer")
                .SetResizable(true)
                .SetSize(1280, 900);

           //  _window.Load(new Uri($"http://localhost:{_server.Port}"));
            _window.Load("https://www.google.com");
           //  window.WaitForClose();
        
    }
}