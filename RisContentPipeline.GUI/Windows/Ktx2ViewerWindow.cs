using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;
using RisContentPipeline.GUI.Controls;


namespace RisContentPipeline.GUI.Windows
{
    internal class Ktx2ViewerWindow : Form
    {
        private readonly WebView _webView;
        private Bitmap? _currentImage;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal Ktx2ViewerWindow(Context context)
        {
            string contentFolder = Path.Combine(AppContext.BaseDirectory, "Ktx2BrowserViewerContent");
            var server = new LocalWebServer(contentFolder, port: 8080);
            _ = server.StartAsync();

            Title = "Image Viewer";

            Resizable = true;
            ClientSize = new Size(800, 600);

            context.OnItemSelected += item =>
            {
                // if (item.Image != null)
                // {
                //     _currentImage = new Bitmap(item.Image.Data);
                //
                //     if (_imageView != null)
                //     {
                //         _imageView.Image = _currentImage;
                //     }
                // }
            };

            var htmlFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Ktx2BrowserViewerContent",
                "index.html");

            _webView = new WebView()
            {
                Size = new Size(400, 400),
                Url = new Uri("http://localhost:8080"),
            };

            string filePath = "/home/luka/Desktop/test_khr.ktx2";
            byte[] data = File.ReadAllBytes(filePath);

            string base64 = Convert.ToBase64String(data);

            _webView.DocumentLoaded += (sender, e) =>
            {
                var result = _webView.ExecuteScript("try{" +
                                                    $"window.loadKtx2TextureFromBase64('{base64}');" +
                                                    "}catch(e){alert(e);}"
                );
                Console.WriteLine(result);
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

        /// <inheritdoc/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _currentImage?.Dispose();
            _currentImage = null;
        }
    }
}