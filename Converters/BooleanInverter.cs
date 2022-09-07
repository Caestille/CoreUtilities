using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// A <see cref="IValueConverter"/> which returns the inverse of the <see cref="bool"/> value provided in the
	/// <see cref="Binding"/>.
	/// </summary>
	public class BooleanInverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}