using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CoreUtilities.Converters
{
	public class StringWidthGetterConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is string text
				&& values[1] is double fontSize
				&& values[2] is FontFamily fontFamily
				&& values[3] is FontStyle fontStyle
				&& values[4] is FontWeight fontWeight
				&& values[5] is FontStretch fontStretch)
			{
				var overrideText = string.Empty;
				var padding = 0d;
				if (parameter is string param)
				{
					var array = param.Split('|');
					overrideText = array[0];
					if (array.Length > 1 && double.TryParse(array[1], out double value))
					{
						padding = value;
					}
				}
				double width = MeasureString(!string.IsNullOrEmpty(overrideText) ? overrideText : text, fontSize, fontFamily, fontStyle, fontWeight, fontStretch);
				return width + padding;
			}

			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
		}

		private double MeasureString(string? text, double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch)
		{
			if (text != null)
			{
#pragma warning disable CS0618
				var formattedText = new FormattedText(
#pragma warning restore CS0618
					text,
					CultureInfo.CurrentCulture,
					FlowDirection.LeftToRight,
					new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
					fontSize,
					Brushes.Black);

				return formattedText.Width;
			}

			return 0;
		}
	}
}