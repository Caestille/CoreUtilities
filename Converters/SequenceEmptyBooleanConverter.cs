using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which given an <see cref="IEnumerable{T}"/>, returns a <see cref="bool"/>
	/// indicating whether the <see cref="IEnumerable{T}"/> is empty or not.
	/// </summary>
	public class SequenceEmptyBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value)
			{
				case IEnumerable<object> objectList:
					return !objectList.Any();
				case IEnumerable<int> intList:
					return !intList.Any();
				case IEnumerable<float> floatList:
					return !floatList.Any();
				case IEnumerable<double> doubleList:
					return !doubleList.Any();
				case IEnumerable<bool> boolList:
					return !boolList.Any();
				case IEnumerable<long> longList:
					return !longList.Any();
				case IEnumerable<ulong> ulongList:
					return !ulongList.Any();
				case IEnumerable<short> shortList:
					return !shortList.Any();
				case IEnumerable<ushort> ushortList:
					return !ushortList.Any();
				case IEnumerable<byte> byteList:
					return !byteList.Any();
				case IEnumerable<sbyte> sbyteList:
					return !sbyteList.Any();
				case IEnumerable<char> charList:
					return !charList.Any();
			}

			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}