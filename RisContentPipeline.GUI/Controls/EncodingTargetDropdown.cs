using Eto.Forms;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The dropdown for selecting the encoding target.
/// </summary>
public sealed class EncodingTargetDropdown : DropDown
{
    /// <summary>
    /// The texture modes.
    /// </summary>
    private readonly string[] _encodingTargets = ["No Encoding", "Basis ETC1S", "Basis UASTC", "ASTC4X4"];

    /// <summary>
    /// The encoding targets.
    /// </summary>
    public readonly Ktx2EncodingTarget[] EncodingTargets =
    [
        Ktx2EncodingTarget.NO_ENCODING,
        Ktx2EncodingTarget.BASIS_ETC1S,
        Ktx2EncodingTarget.BASIS_UASTC,
        Ktx2EncodingTarget.ASTC_4X4
    ]; 

    /// <summary>
    /// Invoked when the encoding target is selected.
    /// </summary>
    public event Action<EncodingTargetDropdown, Ktx2EncodingTarget>? OnEncodingTargetSelected;
    
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="selectedTarget">The selected encoding target.</param>
    public EncodingTargetDropdown(Ktx2EncodingTarget selectedTarget)
    {
        var textureMode = _encodingTargets[(int) selectedTarget];
        
        SelectedValue = textureMode;
        DataStore = _encodingTargets;
        ToolTip = "Select the desired texture mode for the KTX2 texture. " +
                  "BasisETC1S will encode the texture using ETC1S compression, " +
                  "BasisUASTC will use UASTC compression, " +
                  "ASTC4X4 will use ASTC4X4 compression, " +
                  "and NoEncoding will process the texture as-is.";

        SelectedIndex = (int) selectedTarget;
        SelectedIndexChanged += (sender, e) =>
        {
            OnEncodingTargetSelected?.Invoke(this, EncodingTargets[SelectedIndex]);
        };
    }
}