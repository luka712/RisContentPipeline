using Eto.Forms;

namespace RisContentPipeline.GUI.Controls;

public class GenerateMipmapsCheckbox : CheckBox
{
    public GenerateMipmapsCheckbox(bool generateMipmaps)
    {
        Text = "Generate Mipmaps";
        Checked = generateMipmaps;
        ToolTip = "If enabled, mip levels will be generated for the exported texture.";
    }
}