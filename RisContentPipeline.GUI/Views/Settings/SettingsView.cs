using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Views.Settings;

namespace RisContentPipeline.GUI.Views;

/// <summary>
/// Displays the editable settings for whichever asset is currently selected in the
/// <see cref="AssetView"/>. The view subscribes to <see cref="Context.OnItemSelected"/>
/// and rebuilds itself whenever the selection changes.
/// </summary>
internal class SettingsView
{
    private readonly Scrollable _scrollable;
    private TableLayout _tableLayout;
    private AssetFileOrFolder? _selectedFileOrFolder;

    /// <summary>
    /// The titled panel that hosts the settings UI.
    /// </summary>
    internal Control Content { get; }

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="context">The <see cref="Context"/>.</param>
    public SettingsView(Context context)
    {
        _tableLayout = CreateTableLayout();
        _tableLayout.Rows.Add(new TableRow(new Label
        {
            Text = "No asset selected.",
            TextColor = SystemColors.DisabledText,
        }));

        _scrollable = new Scrollable
        {
            Border = BorderType.None,
            ExpandContentWidth = true,
            Content = _tableLayout,
        };

        Content = new GroupBox
        {
            Text = "Settings",
            Font = SystemFonts.Bold(),
            Padding = new Padding(Theme.PADDING),
            Content = _scrollable,
        };

        context.OnItemSelected += item => FileOrFolder = item;
    }

    private static TableLayout CreateTableLayout()
        => new()
        {
            Padding = new Padding(Theme.PADDING),
            Spacing = Theme.FormSpacing,
            Width = Theme.RIGHT_SIDE_PANELS_MIN_WIDTH - Theme.PADDING * 2 - Theme.FormSpacing.Width * 2,
        };

    /// <summary>
    /// The selected <see cref="AssetFileOrFolder"/> object.
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
        _tableLayout = CreateTableLayout();

        if (_selectedFileOrFolder?.Image != null)
        {
            ImageAssetProperties(_selectedFileOrFolder.Image);
        }
        else if (_selectedFileOrFolder != null)
        {
            _tableLayout.Rows.Add(new TableRow(
                new Label { Text = "Name", TextColor = SystemColors.ControlText },
                new Label { Text = _selectedFileOrFolder.PathOrFileName ?? string.Empty }
            ));
            _tableLayout.Rows.Add(new TableRow(
                new Label { Text = "Path", TextColor = SystemColors.ControlText },
                new Label { Text = _selectedFileOrFolder.AbsolutePathOrFileName ?? string.Empty, Wrap = WrapMode.Word }
            ));
        }
        else
        {
            _tableLayout.Rows.Add(new TableRow(new Label
            {
                Text = "No asset selected.",
                TextColor = SystemColors.DisabledText,
            }));
        }

        // Push the rows to the top, leaving empty space at the bottom.
        _tableLayout.Rows.Add(new TableRow { ScaleHeight = true });

        _scrollable.Content = _tableLayout;
    }

    private void ImageAssetProperties(ImageContainer imageContainer)
    {
        _tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Name", TextColor = SystemColors.ControlText},
            new Label { Text = imageContainer.FileName }
        ));

        PngSettingsView.Create(imageContainer, _tableLayout);
    }
}
