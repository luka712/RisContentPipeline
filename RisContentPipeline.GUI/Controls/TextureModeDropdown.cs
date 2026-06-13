using Eto.Forms;
using RisContentPipeline.GUI.Settings;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.Controls;

public class TextureModeDropdown : DropDown
{
    private readonly string[] _textureModes = ["No Encoding", "Basis ETC1S", "Basis UASTC", "ASTC4X4"];
    
    public TextureModeDropdown(Ktx2EncodingTarget selectedTarget)
    {
        var textureMode = _textureModes[(int) selectedTarget];
        
        SelectedValue = textureMode;
        DataStore = _textureModes;
        ToolTip = "Select the desired texture mode for the KTX2 texture. " +
                  "BasisETC1S will encode the texture using ETC1S compression, " +
                  "BasisUASTC will use UASTC compression, " +
                  "ASTC4X4 will use ASTC4X4 compression, " +
                  "and NoEncoding will process the texture as-is.";

        SelectedIndex = (int) selectedTarget;
        SelectedIndexChanged += (sender, e) =>
        {
            // imageContainer.Ktx2ExportSettings.EncodeTarget = (Ktx2EncodingTarget)textureModeDropdown.SelectedIndex;
        };
        
        // tableLayout.Rows.Add(new TableRow(
        //     new Label { Text = "Texture Mode" },
        //     new TableCell(textureModeDropdown, scaleWidth: true)
        // ));
    }
}