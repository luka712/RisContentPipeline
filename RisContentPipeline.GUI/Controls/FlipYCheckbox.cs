using Eto.Forms;

namespace RisContentPipeline.GUI.Controls;

// TODO:
public class FlipYCheckbox : CheckBox
{
    // TODO:
    public FlipYCheckbox(bool flipY)
    {
        Text = "Flip Y";
        Checked = flipY;
        ToolTip = "If true, the generated texture will be fliped.";
    }
}