using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	public class ValueAdderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Max(0, (double)value + (double)System.Convert.ChangeType(parameter, typeof(double)));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Max(0, (double)value - (double)System.Convert.ChangeType(parameter, typeof(double)));
		}
	}
}