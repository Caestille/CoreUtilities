using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which when given a <see cref="string"/>, if not empty, returns
	/// <see cref="Visibility.Visible"/>, else <see cref="Visibility.Collapsed"/>. A converter parameter can be
	/// optionally specified as <see cref="true"/> or <see cref="false"/> to invert the result.
	/// </summary>
	public class StringNotEmptyVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is not string)
				return Visibility.Visible;

			var invert = false;
			if (parameter is string boolString && bool.TryParse(boolString, out var doInvert)) invert = doInvert;

			var empty = value == null || string.IsNullOrEmpty((string)value);
			if (invert) empty = !empty;

			return empty ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}