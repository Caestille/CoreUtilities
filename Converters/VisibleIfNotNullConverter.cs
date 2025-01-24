namespace CoreUtilities.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// An <see cref="IValueConverter"/> which returns <see cref="Visibility.Visible"/> if the value provided in the
/// <see cref="Binding"/> is not <see langword="null"/>, else <see cref="Visibility.Collapsed" />.
/// </summary>
public class VisibleIfNotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value == null) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
