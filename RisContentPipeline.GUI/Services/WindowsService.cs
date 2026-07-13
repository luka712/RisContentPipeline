using Avalonia.Controls;
using RisContentPipeline.GUI.Windows;
using SukiUI.Controls;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// The window service.
/// </summary>
public class WindowsService
{
    private const string X11_WINDOW_HANDLE_DESCRIPTOR = "XID";

    private readonly List<SukiWindow> _windows = new();

    /// <summary>
    /// Gets or sets a value indicating whether to use the native title bar.
    /// </summary>
    public bool NativeTitleBar
    {
        get;
        set
        {
            field = value;
            foreach (var window in _windows)
            {
                ApplySettings(window);
            }
        }
    }

    /// <summary>
    /// Adds a window to the window service.
    /// </summary>
    /// <param name="window">The window.</param>
    public void AddWindow(SukiWindow window)
    {
        if (!_windows.Contains(window))
        {
            _windows.Add(window);
            ApplySettings(window);
        }

        window.Unloaded += (s, e) => _windows.Remove(window);
    }

    /// <summary>
    /// Applies the settings to the windows managed by the window service.
    /// </summary>
    /// <param name="window">The window.</param>
    public void ApplySettings(SukiWindow window)
    {
        // First check do we need a change at all.
        var sukiTitleBarVisible = !NativeTitleBar;
        var windowDecorations = NativeTitleBar ? WindowDecorations.Full : WindowDecorations.BorderOnly;

        if (window.IsTitleBarVisible == sukiTitleBarVisible && window.WindowDecorations == windowDecorations)
        {
            return;
        }

        window.IsTitleBarVisible = sukiTitleBarVisible;
        window.WindowDecorations = windowDecorations;

        /* BUG: X11 cannot reliably distinguish between user-initiated and
           programmatic window resizing. User resizing is disabled to avoid
           incorrect resize handling. */
        if (window is PreferencesWindow)
        {
            var handle = window.TryGetPlatformHandle();
            if (handle?.HandleDescriptor == X11_WINDOW_HANDLE_DESCRIPTOR)
            {
                window.CanResize = false;
            }
        }

        window.SizeToContent = SizeToContent.WidthAndHeight;
        window.InvalidateMeasure();
        window.UpdateLayout();
    }
}