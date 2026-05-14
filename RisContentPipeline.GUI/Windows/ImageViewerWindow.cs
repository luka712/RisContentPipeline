using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;
using RisContentPipeline.GUI.Controls;
using RisContentPipeline.GUI.Data;


namespace RisContentPipeline.GUI.Windows
{
    internal class ImageViewerWindow : Form
    {
        private readonly WebView _webView;
        private readonly LocalWebServer _server;
        private bool _documentLoaded;

        private Action? _documentLoadAction;
        
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal ImageViewerWindow(Context context)
        {
            string contentFolder = Path.Combine(AppContext.BaseDirectory, "Ktx2BrowserViewerContent");
            _server = new LocalWebServer(contentFolder, port: 8080);
            _ = _server.StartAsync();

            Title = "Image Viewer";

            Resizable = true;
            ClientSize = new Size(1000, 800);
            
            _webView = new WebView()
            {
                Size = new Size(400, 400),
                Url = new Uri("http://localhost:8080"),
            };
            
            _webView.DocumentLoaded += (sender, e) =>
            {
                _documentLoaded = true;
                _documentLoadAction?.Invoke();
            };

            // --- Main Layout ---
            Content = new DynamicLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    _webView
                }
            };
        }

        private AssetFileOrFolder? _imageItem;

        private Task LoadImageAsync(AssetFileOrFolder item)
        {
            return Task.Run(() =>
            {
                string filePath = item.AbsolutePathOrFileName!;
                byte[] data = File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(data);
                Application.Instance.Invoke(() =>
                {
                    if (!_documentLoaded)
                    {
                       _documentLoadAction = () => _webView.ExecuteScript($"window.loadPngTextureFromBase64('{base64}');");
                    }
                    else
                    {
                        _webView.ExecuteScript($"window.loadPngTextureFromBase64('{base64}');");
                    }
                });
            });
        }
        
        public AssetFileOrFolder? ImageItem
        {
            get => _imageItem;
            set
            {
                if (value?.IsImage == true)
                {
                    _imageItem = value;
                    _ = LoadImageAsync(_imageItem);
                }
                else
                {
                    throw new InvalidOperationException("ImageItem must be an image.");
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _webView.Dispose();
            _server.Dispose();
        }
    }
}