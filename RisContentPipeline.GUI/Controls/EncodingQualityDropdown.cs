using Eto.Forms;
using RisContentPipeline.GUI.Settings;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The dropdown for selecting the desired encoding quality for KTX2 texture.
/// This is a generic quality dropdown that can be used for any image format.
/// The actual value should be interpreted by the caller.
/// For KTX2, the value should be interpreted as a basis universal compression level.
/// - if ETC1S, the value should be interpreted as a basis universal compression level in range [0, 255].
/// - if UASTC, the value should be interpreted as a basis universal compression level in range [0, 5].
/// </summary>
public class EncodingQualityDropdown : DropDown
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="selected">The selected quality.</param>
    public EncodingQualityDropdown(Ktx2EncodingQuality selectedQuality)
    {
        DataStore = ["Lowest", "Low", "Medium", "High", "Best"];
        SelectedIndex = (int) selectedQuality;
        ToolTip = "Select the desired encoding quality for the KTX2 texture.";

        SelectedIndexChanged += (o, e) =>
        {
            EncodingQualityLevelChanged?.Invoke(this, (Ktx2EncodingQuality) SelectedIndex);
        };
    }

    public event EventHandler<Ktx2EncodingQuality> EncodingQualityLevelChanged;
}