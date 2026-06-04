using Eto.Forms;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The label for the encoding quality setting.
/// </summary>
public class EncodingQualityLabel : Label
{
    /// <summary>
    /// Creates a new instance of the <see cref="EncodingQualityLabel"/> class.
    /// </summary>
    public EncodingQualityLabel()
    {
        Text = "Encoding Quality";
        ToolTip = "Select the desired encoding quality for the KTX2 texture.";
    }
}