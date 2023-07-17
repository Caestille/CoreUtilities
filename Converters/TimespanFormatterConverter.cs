using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreUtilities.Converters
{
	/// <summary>
	/// An <see cref="IValueConverter"/> which when given a <see cref="TimeSpan"/>, formats it nicely.
	/// </summary>
	public class TimespanFormatterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TimeSpan ts)
			{
				var days = ts.Days;
                var hours = ts.Hours;
                var mins = ts.Minutes;
                var secs = ts.Seconds;
                if (days > 0)
				{
					return $"{days.ToString().TrimStart('0')} {(days > 1 ? "days" : "day")}";
				}
				else if (hours > 0)
				{
                    return $"{hours.ToString().TrimStart('0')} {(hours > 1 ? "hours" : "hour")}";
                }
				else if (mins > 0)
				{
                    return $"{mins.ToString().TrimStart('0')} {(mins > 1 ? "minutes" : "minute")}";
                }
				else if (secs > -1)
				{
					return $"{(secs == 0 ? "0" : secs.ToString().TrimStart('0'))} s";
				}
				else
				{
					return $"-{ts.ToString("dd\\ \\d\\a\\y\\s\\ hh\\:mm\\:ss")}";
				}
			}

			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}