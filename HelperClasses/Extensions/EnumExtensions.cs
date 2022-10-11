using System;
using System.ComponentModel;
using System.Reflection;

namespace CoreUtilities.HelperClasses.Extensions
{
    /// <summary>
    /// Extensions for a <see cref="Enum"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description attribute for a given <see cref="Enum"/> if it exists, else the enum name itself as a 
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="enumObj">The <see cref="Enum"/> to get the description of.</param>
        /// <returns>A <see cref="string"/> which is the <see cref="Enum"/> description if it exists, else the
        /// enum value itself as a string.</returns>
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
