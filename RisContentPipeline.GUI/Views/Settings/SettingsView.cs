using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Views.Settings;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// The properties of a selected object.
/// </summary>
internal class SettingsView
{
    private readonly Context _context;
    private TableLayout _tableLayout;
    private AssetFileOrFolder? _selectedFileOrFolder;

    /// <summary>
    /// The tree view that displays the properties of selected <see cref="FileOrFolder"/> object.
    /// </summary>
    internal GroupBox Content { get; }


    /// <summary>
    /// The constructor.
    /// <param name="context">The <see cref="Context"/>.</param>"
    /// </summary>
    public SettingsView(Context context)
    {
        _tableLayout = CreateTableLayout();

        Content = new GroupBox
        {
            Text = "settings",
            Font = SystemFonts.Bold(),
            Padding = new Padding(Theme.PADDING),
            Width = Theme.SIDE_PANELS_WIDTH - Theme.PADDING * 2,
            Content = _tableLayout,
        };

        context.OnItemSelected += item => FileOrFolder = item;
    }

    private TableLayout CreateTableLayout()
        => new ()
        {
            Size = new Size(-1, -1),
            Spacing = new Size(10, 6),
        };

    /// <summary>
    /// The selected <see cref="FileOrFolder"/> object.
    /// </summary>
    public AssetFileOrFolder? FileOrFolder
    {
        get => _selectedFileOrFolder;
        set
        {
            _selectedFileOrFolder = value;
            Refresh();
        }
    }

    public void Refresh()
    {
        if (_selectedFileOrFolder?.Image != null)
        {
            ImageAssetProperties(_selectedFileOrFolder?.Image);
        }
    }

    private void ImageAssetProperties(ImageContainer imageContainer)
    {
        _tableLayout = CreateTableLayout();

        _tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Name" },
            new Label { Text = imageContainer.FileName }
        ));

        
        PngSettingsView.Create(imageContainer, _tableLayout);

        Content.Content = _tableLayout;
    }
}