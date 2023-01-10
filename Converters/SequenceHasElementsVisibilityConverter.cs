using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which given an <see cref="IEnumerable{T}"/>, returns
	/// <see cref="Visibility.Visible"/> if the <see cref="IEnumerable{T}"/> has any elements, else
	/// <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public class SequenceHasElementsVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value)
			{
				case IEnumerable<object> objectList:
					return objectList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<int> intList:
					return intList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<float> floatList:
					return floatList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<double> doubleList:
					return doubleList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<bool> boolList:
					return boolList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<long> longList:
					return longList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<ulong> ulongList:
					return ulongList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<short> shortList:
					return shortList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<ushort> ushortList:
					return ushortList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<byte> byteList:
					return byteList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<sbyte> sbyteList:
					return sbyteList.Any() ? Visibility.Visible : Visibility.Collapsed;
				case IEnumerable<char> charList:
					return charList.Any() ? Visibility.Visible : Visibility.Collapsed;
			}

			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}