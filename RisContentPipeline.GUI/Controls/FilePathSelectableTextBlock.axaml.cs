using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input.Platform;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The file path info text block.
/// </summary>
public class FilePathSelectableTextBlock : TemplatedControl
{
    /// <summary>
    /// The file path property.
    /// </summary>
    public static readonly StyledProperty<string> FilePathProperty =
        AvaloniaProperty.Register<FilePathSelectableTextBlock, string>(nameof(FilePath), defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// The file path
    /// </summary>
    public string FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }
    
    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        var icon = e.NameScope.Find<Button>("PartButton");
        icon?.Click += (_, _) =>
        {
            var filePath = e.NameScope.Find<SelectableTextBlock>("PartSelectableTextBlock")?.Text;

            if (!string.IsNullOrEmpty(filePath))
            {
                var topLevel = TopLevel.GetTopLevel(this);
                _ = topLevel?.Clipboard?.SetTextAsync(filePath);
            }
        };

    }
}