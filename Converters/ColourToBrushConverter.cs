using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// A <see cref="IValueConverter"/> which returns a <see cref="SolidColorBrush"/> whos colour is the colour set in
	/// the binding.
	/// </summary>
	public class ColourToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color colour = value is Color ? (Color)value : Colors.Black;
			return Application.Current.Dispatcher.Invoke(() => new SolidColorBrush(colour));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((SolidColorBrush)value).Color;
		}
	}
}