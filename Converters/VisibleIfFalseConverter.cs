using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which returns <see cref="Visibility.Visible"/> if the value provided in the
	/// <see cref="Binding"/> is <see cref="false"/>, else <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class VisibleIfFalseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}