using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// A <see cref="IValueConverter"/> which returns the opposite <see cref="Visibility"/> of the given
	/// <see cref="Visibility"/> value.
	/// </summary>
	public class VisibilityInverterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
