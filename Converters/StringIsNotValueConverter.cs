using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which given a <see cref="string"/> and a <see cref="string"/> in the converter
	/// parameter, returns a <see cref="bool"/> indicating whether the <see cref="string"/> does not match the
	/// parameter.
	/// </summary>
	public class StringIsNotValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (string)value != (string)parameter;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}