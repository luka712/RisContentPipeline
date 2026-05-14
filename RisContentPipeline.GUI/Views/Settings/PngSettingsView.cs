using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Settings;
using RisContentPipeline.GUI.TreeGridItems;

namespace RisContentPipeline.GUI.Views.Settings;

public class PngSettingsView
{
    private static readonly Dictionary<int, Ktx2BasisUEncodingQuality> _encodingQualityMap = new()
    {
        { 0, Ktx2BasisUEncodingQuality.Lowest },
        { 1, Ktx2BasisUEncodingQuality.Low },
        { 2, Ktx2BasisUEncodingQuality.Medium },
        { 3, Ktx2BasisUEncodingQuality.High },
        { 4, Ktx2BasisUEncodingQuality.Best },
    };
    
    /// <summary>
    /// Adds rows describing the supplied <paramref name="imageContainer"/> to the
    /// given <paramref name="tableLayout"/>. Includes editable export settings on
    /// top and read-only metadata at the bottom.
    /// </summary>
    /// <param name="imageContainer">The <see cref="ImageContainer"/>.</param>
    /// <param name="tableLayout">The table to attach rows to.</param>
    static internal void Create(ImageContainer imageContainer, TableLayout tableLayout)
    {
        // ----- Editable export settings ------------------------------------
        AddSectionHeader(tableLayout, "Export");

        // TEXTURE MODE DROPDOWN
        string[] textureModes = [ "Basis ETC1S", "Basis UASTC"];
        var textureMode = imageContainer.Ktx2ExportSettings.UseUastc ? textureModes[0] : textureModes[1];
        var textureModeDropdown = new DropDown
        {
            SelectedValue = textureMode,
            DataStore = textureModes,
            ToolTip = "Select the desired texture mode for the KTX2 texture. " +
                      "BasisETC1S will encode the texture using ETC1S compression, " +
                      "BasisUASTC will use UASTC compression, " +
                      "and NoEncoding will process the texture as-is.",
        };
        textureModeDropdown.SelectedIndexChanged += (sender, e) =>
        {
            imageContainer.Ktx2ExportSettings.UseUastc = textureModeDropdown.SelectedIndex == 0;
        };
        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Texture Mode" },
            new TableCell(textureModeDropdown, scaleWidth: true)
        ));

        // Encoding Quality
        if (textureMode == textureModes[0] || textureMode == textureModes[1])
        {
            var qualityDropDown = new DropDown();
            qualityDropDown.DataStore =
            [
                Ktx2BasisUEncodingQuality.Lowest,
                Ktx2BasisUEncodingQuality.Low,
                Ktx2BasisUEncodingQuality.Medium,
                Ktx2BasisUEncodingQuality.High,
                Ktx2BasisUEncodingQuality.Best
            ];
            qualityDropDown.SelectedIndex = 2;
            qualityDropDown.SelectedIndexChanged += (sender, e) =>
            {
                imageContainer.Ktx2ExportSettings.QualityLevel = _encodingQualityMap[qualityDropDown.SelectedIndex];
            };

            tableLayout.Rows.Add(new TableRow(
                new Label() { Text = "Encoding Quality" },
                qualityDropDown
            ));
        }

        // GENERATE MIPMAPS
        var generateMipmapsCheckBox = new CheckBox
        {
            Text = "Generate Mipmaps",
            Checked = imageContainer.Ktx2ExportSettings.GenerateMipmaps,
            ToolTip = "If enabled, mip levels will be generated for the exported texture.",
        };
        generateMipmapsCheckBox.CheckedChanged += (sender, e) =>
            imageContainer.Ktx2ExportSettings.GenerateMipmaps = generateMipmapsCheckBox.Checked == true;
        tableLayout.Rows.Add(new TableRow(
            new Label(),
            generateMipmapsCheckBox
        ));

        // ----- Read-only metadata ------------------------------------------
        AddSectionHeader(tableLayout, "Meta");

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "File Path", VerticalAlignment = VerticalAlignment.Top  },
            new Label
            {
                Text = imageContainer.FilePath,
                Wrap = WrapMode.Word,
                TextColor = SystemColors.DisabledText,
            }
        ));

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Type"},
            new Label
            {
                Text = imageContainer.FileType,
                TextColor = SystemColors.DisabledText,
            }
        ));
    }

    /// <summary>
    /// Adds a bold separator-style header row to the given table.
    /// </summary>
    private static void AddSectionHeader(TableLayout tableLayout, string title)
    {
        tableLayout.Rows.Add(new TableRow(new TableCell(new Label
        {
            Text = title,
            Font = SystemFonts.Bold(),
            Width = Theme.SETTINGS_LABEL_COLUMN_WIDTH,
        }, scaleWidth: false)));
    }
}
