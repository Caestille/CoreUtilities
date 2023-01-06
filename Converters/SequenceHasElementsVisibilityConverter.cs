using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which given an <see cref="IEnumerable{T}"/>, returns
	/// <see cref="Visibility.Visible"/> if the <see cref="IEnumerable{T}"/> has any elements, else
	/// <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class SequenceHasElementsVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<object> list = (IEnumerable<object>)value;
			return list.Any() ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}