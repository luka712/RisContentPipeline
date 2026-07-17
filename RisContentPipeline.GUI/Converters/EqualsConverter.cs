using System.Globalization;
using Avalonia.Data.Converters;
using Microsoft.Extensions.Logging;
using RisContentPipeline.GUI.Models;

namespace RisContentPipeline.GUI.Converters;

    /// <summary>
    /// Converter that checks if two values are equal. Special-cased for <see cref="MessageLogLevel"/> to support log filtering in UI.
    /// Used in XAML for visibility or style triggers based on log level.
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
