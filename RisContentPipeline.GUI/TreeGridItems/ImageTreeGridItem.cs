using Eto.Forms;
using Eto.Drawing;
using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.TreeGridItems;

/// <summary>
/// Represents a tree grid item that displays an image thumbnail and title.
/// </summary>
internal class ImageTreeGridItem : TreeGridItem
{
    /// <summary>
    /// Gets or sets the image to display as a thumbnail.
    /// </summary>
    public Image? Image { get; set; }

    /// <summary>
    /// Gets or sets the title text to display.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the file path associated with this item.
    /// </summary>
    public FileOrFolder? FileOrFolder { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTreeGridItem"/> class.
    /// </summary>
    /// <param name="title">The title to display.</param>
    /// <param name="fileOrFolder">The file or folder of this item.</param>
    /// <param name="icon">Icon display as a thumbnail.</param>
    public ImageTreeGridItem(string title, FileOrFolder? fileOrFolder, Icon icon)
    {
        Title = title;
        Image = icon;
        FileOrFolder = fileOrFolder;
        
        // Set the Values array with image and title for the ImageTextCell
        Values = new object[] { Image ?? new Bitmap(16, 16, PixelFormat.Format32bppRgba), Title };
    }

    /// <summary>
    /// Updates the image and refreshes the display.
    /// </summary>
    /// <param name="newImage">The new image to display.</param>
    public void UpdateImage(Image newImage)
    {
        Image = newImage;
        Values = new object[] { Image, Title };
    }

    /// <summary>
    /// Updates the title and refreshes the display.
    /// </summary>
    /// <param name="newTitle">The new title to display.</param>
    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
        Values = new object[] { Image ?? new Bitmap(16, 16, PixelFormat.Format32bppRgba), Title };
    }
}
