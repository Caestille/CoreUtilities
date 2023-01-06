using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which multiples a given <see cref="double"/> by the given converter parameter
	/// </summary>
	public class ValueMultiplierConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value * (double)System.Convert.ChangeType(parameter, typeof(double));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value * (1d / (double)System.Convert.ChangeType(parameter, typeof(double)));
		}
	}
}