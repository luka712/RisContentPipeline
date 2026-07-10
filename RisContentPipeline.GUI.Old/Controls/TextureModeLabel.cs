using Eto.Forms;

namespace RisContentPipeline.GUI.Controls;

public class TextureModeLabel : Label
{
    public TextureModeLabel()
    {
        Text = "Texture Mode";
        ToolTip = "The texture mode to use for the asset. 0 = No Encoding, 1 = Basis ETC1S, 2 = Basis UASTC";
    }
}