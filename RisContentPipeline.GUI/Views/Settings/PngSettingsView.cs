using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Views.Settings;

public class PngSettingsView
{
    /// <summary>
    /// Adds rows describing the supplied <paramref name="imageContainer"/> to the
    /// given <paramref name="tableLayout"/>. Includes editable export settings on
    /// top and read-only metadata at the bottom.
    /// </summary>
    /// <param name="imageContainer">The <see cref="ImageContainer"/>.</param>
    /// <param name="tableLayout">The table to attach rows to.</param>
    static internal void Create(Context context, ImageContainer imageContainer, TableLayout tableLayout)
    {
        // Defaults
        int encodingIndex = (int)context.Preferences.Ktx2GlobalSettings.EncodeTarget;
        bool generateMipmaps = context.Preferences.Ktx2GlobalSettings.GenerateMipmaps;
        int qualityLevelIndex =
            Ktx2SettingsLookup.GetIndex(context.Preferences.Ktx2GlobalSettings.QualityLevel);

        // ----- Editable export settings ------------------------------------
        AddSectionHeader(tableLayout, "Export");

        // TEXTURE MODE DROPDOWN
        string[] textureModes = ["No Encoding", "Basis ETC1S", "Basis UASTC"];
        var textureMode = textureModes[encodingIndex];
        var textureModeDropdown = new DropDown
        {
            SelectedValue = textureMode,
            DataStore = textureModes,
            ToolTip = "Select the desired texture mode for the KTX2 texture. " +
                      "BasisETC1S will encode the texture using ETC1S compression, " +
                      "BasisUASTC will use UASTC compression, " +
                      "and NoEncoding will process the texture as-is.",
        };
        textureModeDropdown.SelectedIndex = encodingIndex;
        textureModeDropdown.SelectedIndexChanged += (sender, e) =>
        {
            imageContainer.Ktx2ExportSettings.EncodeTarget = (Ktx2EncodingTarget)textureModeDropdown.SelectedIndex;
        };
        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Texture Mode" },
            new TableCell(textureModeDropdown, scaleWidth: true)
        ));

        // Encoding Quality

        var qualityDropDown = new DropDown();
        qualityDropDown.DataStore = ["Lowest", "Low", "Medium", "High", "Highest"];
        qualityDropDown.SelectedIndex = qualityLevelIndex;
        qualityDropDown.SelectedIndexChanged += (sender, e) =>
        {
            imageContainer.Ktx2ExportSettings.QualityLevel =
                Ktx2SettingsLookup.GetEncodingQualityLevel(qualityDropDown.SelectedIndex);
        };

        tableLayout.Rows.Add(new TableRow(
            new Label() { Text = "Encoding Quality" , ToolTip = "Select the desired encoding quality for the KTX2 texture."},
            qualityDropDown
        ));
        
        // GENERATE MIPMAPS
        var generateMipmapsCheckBox = new CheckBox
        {
            Text = "Generate Mipmaps",
            Checked = generateMipmaps,
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
            new Label { Text = "File Path", VerticalAlignment = VerticalAlignment.Top },
            new Label
            {
                Text = imageContainer.FilePath,
                Wrap = WrapMode.Word,
                TextColor = SystemColors.DisabledText,
            }
        ));

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Type" },
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