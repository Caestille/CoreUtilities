namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// An <see cref="IValueConverter"/> which when given an <see cref="object"/>, returns a <see cref="bool"/>
/// indicating whether the <see cref="object"/> is <see langword="null"/> or not.
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
