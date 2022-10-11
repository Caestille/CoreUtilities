using CoreUtilities.HelperClasses.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
    public class EnumDescriptionGetterConverter : IValueConverter
	{
		Enum cachedEnum;

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Enum enumObject)
			{
				cachedEnum = enumObject;
				return enumObject.GetEnumDescription();
			}

			return Binding.DoNothing;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string description)
			{
				if (description == cachedEnum.GetEnumDescription())
				{
					return cachedEnum;
				}
				else
				{
					return Binding.DoNothing;
				}
			}

			return Binding.DoNothing;
		}
	}
}