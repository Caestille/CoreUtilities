using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IMultiValueConverter"/> which given a <see cref="bool"/> as item 1, and two other bindings in
	/// items 2 and 3, returns items 2 if true, and item 3 if false
	/// </summary>
	public class ValueIfTrueValueIfFalseConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is bool value && value)
			{
				return values[1];
			}
			else
			{
				return values[2];
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}