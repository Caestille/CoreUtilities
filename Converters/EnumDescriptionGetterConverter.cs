using CoreUtilities.HelperClasses;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	public class EnumDescriptionGetterConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Enum enumObject)
				return enumObject.GetEnumDescription();

			return Binding.DoNothing;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}

		
	}
}