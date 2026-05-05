using Eto.Drawing;
using Eto.Forms;
using RisContentPipeline.GUI.Data;

namespace RisContentPipeline.GUI.Views.Settings;

public class PngSettingsView
{
    /// <summary>
    /// Parent is a table to which we assign various settings.
    /// </summary>
    /// <param name="imageContainer">The <see cref="ImageContainerExtended"/>.</param>
    /// <param name="tableLayout">The table to attach rows to.</param>
    static internal void Create(ImageContainerExtended imageContainer, TableLayout tableLayout)
    {
        // TEXTURE MODE DROPDOWN
        string[] textureModes = ["Basis UASTC", "Basis ETC1S"];
        var textureMode = imageContainer.Ktx2ExportSettings.UseUastc ? textureModes[0] : textureModes[1];
        var textureModeDropdown = new DropDown
        {
            SelectedValue = textureMode,
            DataStore = textureModes,
            ToolTip = "Select the desired texture mode for the KTX2 texture." +
                      " BasisETC1S will encode the texture using ETC1S compression," +
                      " BasisUASTC will use UASTC compression, " +
                      "and NoEncoding will process the texture as-is.",
        };
        textureModeDropdown.SelectedIndexChanged += (sender, e) =>
        {
            imageContainer.Ktx2ExportSettings.UseUastc = textureModeDropdown.SelectedIndex == 0;   
        };
        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Desired Texture Mode" },
            textureModeDropdown
        ));

        // GENERATE MIPMAPS
        var generateMipmapsCheckBox = new CheckBox { Text = "Generate Mipmaps" };
        generateMipmapsCheckBox.Checked = imageContainer.Ktx2ExportSettings.GenerateMipmaps;
        generateMipmapsCheckBox.CheckedChanged += (sender, e) =>
            imageContainer.Ktx2ExportSettings.GenerateMipmaps = generateMipmapsCheckBox.Checked == true;
        tableLayout.Rows.Add(new TableRow(
            generateMipmapsCheckBox
        ));

        // META
        tableLayout.Rows.Add(new TableRow(
            new Label() { Text = "Meta", Font = SystemFonts.Bold() }
        ));

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Dimensions" },
            new Label { Text = $"{imageContainer.Width} x {imageContainer.Height}" }
        ));

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "File Path" },
            new Label { Text = imageContainer.FilePath }
        ));

        tableLayout.Rows.Add(new TableRow(
            new Label { Text = "Type" },
            new Label { Text = imageContainer.FileType }
        ));
    }
}