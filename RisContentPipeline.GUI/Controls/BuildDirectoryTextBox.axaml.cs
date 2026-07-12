using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Material.Icons.Avalonia;

namespace RisContentPipeline.GUI.Controls;

/// <summary>
/// The text box for selecting the build directory.
/// </summary>
public partial class BuildDirectoryTextBox : TemplatedControl
{
    /// <summary>
    /// The build directory property.
    /// </summary>
    internal static readonly StyledProperty<string> BuildDirectoryProperty =
        AvaloniaProperty.Register<BuildDirectoryTextBox, string>(nameof(BuildDirectory));

    /// <summary>
    /// The build directory.
    /// </summary>
    public string BuildDirectory
    {
        get => GetValue(BuildDirectoryProperty);
        set => SetValue(BuildDirectoryProperty, value);
    }
    
    /// <summary>
    /// The button icon property.
    /// </summary>
    internal static readonly StyledProperty<bool> IsButtonIconProperty 
        = AvaloniaProperty.Register<BuildDirectoryTextBox, bool>(nameof(IsButtonIcon), defaultBindingMode: BindingMode.OneTime, defaultValue: true);

    /// <summary>
    /// If true, the button will be an icon.
    /// </summary>
    public bool IsButtonIcon
    {
        get => GetValue(IsButtonIconProperty);
        set => SetValue(IsButtonIconProperty, value);
    }

    /// <summary>
    /// Selects a top folder.
    /// </summary>
    private async Task SelectFolderAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Build Directory",
            AllowMultiple = false
        });

        var directory = folders.FirstOrDefault()?.Path.LocalPath;
        if (directory is not null)
        {
            BuildDirectory = directory;
        }
    }
    
    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        var btn = e.NameScope.Find<Button>("PartButton");
        btn?.Click += (_, _) => _ = SelectFolderAsync();

        var icon = e.NameScope.Find<Button>("PartIconButton");
        icon?.Click += (_, _) => _ = SelectFolderAsync();

    }
}