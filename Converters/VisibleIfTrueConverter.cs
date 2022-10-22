using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which returns <see cref="Visibility.Visible"/> if the value provided in the
	/// <see cref="Binding"/> is <see cref="true"/>, else <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class VisibleIfTrueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var visType = parameter != null
				? ((string)parameter) == "Collapsed" ? Visibility.Collapsed : ((string)parameter) == "Hidden" 
					? Visibility.Hidden
					: Visibility.Collapsed
				: Visibility.Collapsed;
			return (bool)value ? Visibility.Visible : visType;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}