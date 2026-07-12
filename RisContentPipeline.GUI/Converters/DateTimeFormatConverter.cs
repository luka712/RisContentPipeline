using System.Globalization;
using Avalonia.Data.Converters;

namespace RisContentPipeline.GUI.Converters;

/// <summary>
/// The converter for formatting a <see cref="DateTime"/> object.
/// </summary>
public class DateTimeFormatConverter : IValueConverter
{
    /// <summary>
    /// The format string to use for formatting the <see cref="DateTime"/> object.
    /// </summary>
    public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";

    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return dt.ToString(Format);

        return "";
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}