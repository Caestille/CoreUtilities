using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which when given an <see cref="object"/>, returns a <see cref="bool"/>
	/// indicating whether the <see cref="object"/> can be unboxed to an <see cref="int"/> or not.
	/// </summary>
	public class IsIntegerConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt32(value) == int.Parse((string)parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
