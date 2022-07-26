using System;
using System.ComponentModel;
using System.Reflection;

namespace CoreUtilities.HelperClasses
{
	public static class EnumExtensions
	{
		public static string GetEnumDescription(this Enum enumObj)
		{
			FieldInfo? fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

			object[] attribArray = fieldInfo!.GetCustomAttributes(false);

			if (attribArray.Length == 0)
			{
				return enumObj.ToString();
			}

			DescriptionAttribute attrib = (attribArray[0] as DescriptionAttribute)!;
			return attrib.Description;
		}
	}
}
