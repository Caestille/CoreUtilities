﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
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