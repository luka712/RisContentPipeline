using System.Globalization;
using Avalonia.Data.Converters;
using Microsoft.Extensions.Logging;
using RisContentPipeline.GUI.Models;

namespace RisContentPipeline.GUI.Converters;

/// <summary>
/// TODO: doc comments
/// </summary>
public class EqualsConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageLogLevel lv && parameter is MessageLogLevel lp)
        {
            return lv == lp;
        }
        
        return value?.Equals(parameter);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}