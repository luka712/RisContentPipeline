using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The checkbox for flipping the texture vertically.
/// </summary>
public class FlipYCheckBox : TemplatedControl
{
    /// <summary>
    /// The property for the checked state.
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<FlipYCheckBox, bool>(nameof(IsChecked));

    /// <summary>
    /// The checked state.
    /// </summary>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);   
    }
}