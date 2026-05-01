using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;


namespace RisContentPipeline.GUI.Windows
{
    internal class ImageViewerWindow : Form
    {
        private readonly ImageView _imageView;
        private Bitmap? _currentImage;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The <see cref="Context"/>.</param>
        internal ImageViewerWindow(Context context)
        {
            Title = "Image Viewer";

            Resizable = true;
            ClientSize = new Size(800, 600);

            context.OnItemSelected += item =>
            {
                if (item.Image != null)
                {
                    _currentImage = new Bitmap(item.Image.Data);

                    if (_imageView != null)
                    {
                        _imageView.Image = _currentImage;
                    }
                }
            };

            _imageView = new ImageView
            {
                Size = new Size(400, 400),
                Image = _currentImage
            };

            // --- Main Layout ---
            Content = new DynamicLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    _imageView
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
