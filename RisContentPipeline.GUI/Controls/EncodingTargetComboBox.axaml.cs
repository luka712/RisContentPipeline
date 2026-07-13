using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using RisContentPipeline.Ktx2;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The combo box for selecting the encoding target.
/// </summary>
public class EncodingTargetComboBox : TemplatedControl
{
    private readonly Ktx2EncodingTarget[] _encodingTargets =
    [
        Ktx2EncodingTarget.NO_ENCODING,
        Ktx2EncodingTarget.BASIS_ETC1S,
        Ktx2EncodingTarget.BASIS_UASTC,
        Ktx2EncodingTarget.ASTC_4X4
    ];

    /// <summary>
    /// The encoding texture modes.
    /// </summary>
    internal static readonly DirectProperty<EncodingTargetComboBox, string[]> EncodingTargetsProperty =
        AvaloniaProperty.RegisterDirect<EncodingTargetComboBox, string[]>(
            nameof(EncodingTargets),
            o => o.EncodingTargets);

    /// <summary>
    /// The encoding texture modes.
    /// </summary>
    internal string[] EncodingTargets { get; } =
    [
        "No Encoding",
        "Basis ETC1S",
        "Basis UASTC",
        "ASTC4X4"
    ];

    /// <summary>
    /// The selected index.
    /// </summary>
    internal static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<EncodingTargetComboBox, int>(nameof(SelectedIndex),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The selected index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// The selected encoding target.
    /// </summary>
    public static readonly StyledProperty<Ktx2EncodingTarget> SelectedEncodingTargetProperty =
        AvaloniaProperty.Register<EncodingTargetComboBox, Ktx2EncodingTarget>(
            nameof(SelectedEncodingTarget),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The selected encoding target.
    /// </summary>
    public Ktx2EncodingTarget SelectedEncodingTarget
    {
        get => GetValue(SelectedEncodingTargetProperty);
        set => SetValue(SelectedEncodingTargetProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedIndexProperty)
        {
            SelectedEncodingTarget = _encodingTargets[SelectedIndex];
        }
        else if (change.Property == SelectedEncodingTargetProperty)
        {
            SelectedIndex = Array.IndexOf(_encodingTargets, SelectedEncodingTarget);
        }
    }
}