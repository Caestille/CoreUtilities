﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
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