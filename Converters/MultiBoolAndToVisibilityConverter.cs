using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// Given a set of <see cref="bool"/> bindings, if an AND operation on all <see cref="bool"/>s evaluates to
	/// <see cref="true"/>, returns <see cref="Visibility.Visible"/>, else <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class MultiBoolAndToVisibilityConverter : IMultiValueConverter
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
			return allow ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { Binding.DoNothing };
		}
	}
}