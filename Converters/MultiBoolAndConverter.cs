using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// Given a set of <see cref="bool"/> bindings, returns the result of an AND operation on all <see cref="bool"/>
	/// values given.
	public class MultiBoolAndConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool allow = true;
			foreach (object value in values)
			{
				if (value is bool castValue)
				{
					allow &= castValue;
				}
			}
			return allow;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { Binding.DoNothing };
		}
	}
}