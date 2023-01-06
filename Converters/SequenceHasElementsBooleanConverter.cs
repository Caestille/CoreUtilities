using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which given an <see cref="IEnumerable{T}"/>, returns a <see cref="bool"/>
	/// indicating whether the <see cref="IEnumerable{T}"/> contains any elements.
	/// </summary>
	public class SequenceHasElementsBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<object> list = (IEnumerable<object>)value;
			return list.Any();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}