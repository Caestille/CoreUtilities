namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// An <see cref="IValueConverter"/> which given a <see cref="string"/>, returns a <see cref="bool"/> indiating
/// whether the <see cref="string"/> is not empty or not.
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !string.IsNullOrEmpty((string)value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
