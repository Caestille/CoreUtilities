namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// A <see cref="IValueConverter"/> which returns <see langword="true"/> if the given value is > 0.
/// </summary>
public class IsGreaterThanZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (double)value > 0;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
