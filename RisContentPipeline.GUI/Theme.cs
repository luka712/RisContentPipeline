using Eto.Drawing;

namespace RisContentPipeline.GUI;

/// <summary>
/// Centralized layout, sizing, and styling constants used by all views and windows.
/// </summary>
internal static class Theme
{
    // ----- Spacing & padding ------------------------------------------------

    /// <summary>Default outer padding for views and panels.</summary>
    internal const int PADDING = 6;

    /// <summary>Default spacing between grouped controls inside a layout.</summary>
    internal const int CONTROL_SPACING = 6;

    /// <summary>Default spacing between sections inside a panel.</summary>
    internal const int SECTION_SPACING = 8;

    // ----- Sizing -----------------------------------------------------------

    /// <summary>Initial width of the main window.</summary>
    internal const int CLIENT_WIDTH = 1280;

    /// <summary>Initial height of the main window.</summary>
    internal const int CLIENT_HEIGHT = 720;

    /// <summary>Minimum width of the left side panels when the user resizes splitters.</summary>
    internal const int LEFT_SIDE_PANELS_MIN_WIDTH = 250;

    /// <summary>Minimum width of the right side panels when the user resizes splitters.</summary>
    internal const int RIGHT_SIDE_PANELS_MIN_WIDTH = 350;

    /// <summary>Initial height for the build output panel.</summary>
    internal const int BUILD_OUTPUT_HEIGHT = 220;

    /// <summary>Default height for action bar buttons.</summary>
    internal const int BUTTON_HEIGHT = 28;

    /// <summary>Default width for labels in settings views.</summary>
    internal const int SETTINGS_LABEL_COLUMN_WIDTH = 120;

    // ----- Layout helpers ---------------------------------------------------

    /// <summary>Default <see cref="Padding"/> instance for views.</summary>
    internal static Padding DefaultPadding => new(PADDING);

    /// <summary>Default <see cref="Size"/> spacing for table layouts.</summary>
    internal static Size DefaultSpacing => new(CONTROL_SPACING, CONTROL_SPACING);

    /// <summary>Slightly tighter spacing for forms.</summary>
    internal static Size FormSpacing => new(8, 4);
}
