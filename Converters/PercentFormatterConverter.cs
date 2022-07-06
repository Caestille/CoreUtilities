﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	public class PercentFormatterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return $"{Math.Round((double) value, 0)} %";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}