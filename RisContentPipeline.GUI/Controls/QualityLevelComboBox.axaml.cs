using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The combo box for selecting the quality level.
/// </summary>
public class QualityLevelComboBox : TemplatedControl
{
    /// <summary>
    /// The quality levels.
    /// </summary>
    public string[] QualityLevels { get; } =
    [
        "Lowest",
        "Low",
        "Medium",
        "High",
        "Best"
    ];

    /// <summary>
    /// The encoding texture modes.
    /// </summary>
    internal static readonly DirectProperty<QualityLevelComboBox, string[]> QualityLevelsProperty =
        AvaloniaProperty.RegisterDirect<QualityLevelComboBox, string[]>(
            nameof(QualityLevels),
            o => o.QualityLevels);
    
    /// <summary>
    /// The selected index.
    /// </summary>
    internal static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<EncodingTargetComboBox, int>(nameof(SelectedIndex));

    /// <summary>
    /// The selected index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }
}