namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// An <see cref="IValueConverter"/> which given a <see cref="string"/> and a <see cref="string"/> in the converter
/// parameter, returns a <see cref="bool"/> indicating whether the <see cref="string"/> matches the parameter.
/// </summary>
public class StringIsValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == (string)parameter;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
