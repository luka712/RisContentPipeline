using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace RisContentPipeline.GUI.Windows
{
    /// <summary>
    /// Creates the browser-based image viewer.
    /// </summary>
    internal class ImageViewerWindow : Form
    {
        private WebView _webView;
        private bool _documentLoaded;
        private Queue<Action?> _documentLoadedCallbacks = new();
        private string _filePath;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal ImageViewerWindow(Context context)
        {
            Title = "Image Viewer";
            Resizable = true;
            ClientSize = new Size(1000, 800);

            _webView = new WebView()
            {
                Size = new Size(400, 400),
                Url = new Uri($"http://localhost:{context.LocalServerPort}"),
            };

            _webView.DocumentLoaded += (sender, e) =>
            {
                _documentLoaded = true;
                while (_documentLoadedCallbacks.Count > 0)
                {
                    _documentLoadedCallbacks.Dequeue()?.Invoke();
                }
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


        /// <summary>
        /// Loads the image into the browser viewer.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private Task LoadImageAsync(string filePath)
        {
            var isKtx2 = filePath.EndsWith(".ktx2");
            var isPng = filePath.EndsWith(".png");

            // js function from viewer to call.
            var jsFunction = isKtx2 ? "loadKtx2TextureFromBase64" : "loadPngTextureFromBase64";
            
            var fileName = Path.GetFileName(filePath);

            return Task.Run(() =>
            {
                byte[] data = File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(data);

                if (!_documentLoaded)
                {
                    var _documentLoadedCallback = () =>
                    {
                        // Wait a bit until script is ready.
                        _webView.ExecuteScript($@"
                        var __tryLoad = function() {{
                            if(!window.{jsFunction}) {{
                                setTimeout(__tryLoad, 250);
                            }}
                            else {{
                                window.{jsFunction}('{base64}', '{fileName}');
                            }}                  
                        }}
                        __tryLoad();
                        ");
                    };

                    _documentLoadedCallbacks.Enqueue(_documentLoadedCallback);
                }
                else
                {
                    // Must be on UI thread.
                    Application.Instance.Invoke(() => _webView.ExecuteScript($"window.{jsFunction}('{base64}');"));
                }
            });
        }


        /// <summary>
        /// Laods collection of images into the browser viewer.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        public void View(string[] filePaths)
        {
            Show();
            foreach (var filePath in filePaths)
            {
                _ = LoadImageAsync(filePath);
            }
        }

        /// <summary>
        /// Laods collection of images into the browser viewer.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void View(string filePath)
        {
            Show();
            _ = LoadImageAsync(filePath);
        }
    }
}