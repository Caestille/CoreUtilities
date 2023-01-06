using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which when given a <see cref="bool"/>, returns a <see cref="DataGridLength"/>
	/// value which will wrap the rows if <see cref="true"/> and set the row width to automatic if <see cref="false"/>.
	/// </summary>
	public class DataGridWrapRowsBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return new DataGridLength(1, DataGridLengthUnitType.Star);
			
			return new DataGridLength(1, DataGridLengthUnitType.Auto);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}