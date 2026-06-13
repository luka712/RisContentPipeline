using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Controls;
using RisContentPipeline.GUI.Data;
using RisContentPipeline.GUI.Settings;
using RisContentPipeline.Ktx2;

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
        var encodingTarget = context.Preferences.Ktx2GlobalSettings.EncodeTarget;
        bool generateMipmaps = context.Preferences.Ktx2GlobalSettings.GenerateMipmaps;
        int qualityLevelIndex = (int) context.Preferences.Ktx2GlobalSettings.QualityLevel;

        // ----- Editable export settings ------------------------------------
        AddSectionHeader(tableLayout, "Export");

        // TEXTURE MODE DROPDOWN
        var textureModeDropdown = new TextureModeDropdown(encodingTarget);
        textureModeDropdown.SelectedIndexChanged += (sender, e) =>
        {
            imageContainer.Ktx2ExportSettings.EncodeTarget = (Ktx2EncodingTarget)textureModeDropdown.SelectedIndex;
        };
        tableLayout.Rows.Add(new TableRow(
            new TextureModeLabel(),
            new TableCell(textureModeDropdown, scaleWidth: true)
        ));

        // Encoding Quality
        var qualityDropDown = new EncodingQualityDropdown(imageContainer.Ktx2ExportSettings.QualityLevel);
        qualityDropDown.EncodingQualityLevelChanged += (sender, quality)
            => imageContainer.Ktx2ExportSettings.QualityLevel = quality;
        tableLayout.Rows.Add(new TableRow(
            new EncodingQualityLabel(),
            qualityDropDown
        ));
        
        // GENERATE MIPMAPS
        var generateMipmapsCheckBox = new GenerateMipmapsCheckbox(generateMipmaps);
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