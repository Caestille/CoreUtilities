namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// An <see cref="IValueConverter"/> which when given a <see cref="double"/>, rounds to 0 decimal places and
/// formats it as a percentage value.
/// </summary>
public class PercentFormatterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{Math.Round((double)value, 0)} %";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
