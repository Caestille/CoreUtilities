using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// A <see cref="IMultiValueConverter"/> which returns <see cref="Visibility.Visible"/> if any of the values in the
	/// given <see cref="Binding"/>s are <see cref="true"/>, else <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class VisibleIfOneTrueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool allow = false;
			foreach (object value in values)
			{
				if (value is bool castValue)
				{
					allow |= castValue;
				}
			}
			return allow ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { Binding.DoNothing, Binding.DoNothing };
		}
	}
}