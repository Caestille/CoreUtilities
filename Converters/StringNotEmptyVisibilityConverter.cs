using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	public class StringNotEmptyVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not string)
				return Visibility.Visible;

			var invert = false;
			if (parameter is string boolString && bool.TryParse(boolString, out var doInvert)) invert = doInvert;

			var empty = string.IsNullOrEmpty((string)value);
			if (invert) empty = !empty;

			return empty ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}