namespace CoreUtilities.Helpers.WPF;

using System;
using System.Globalization;
using System.Windows.Data;

public class InlineMultiConverter : IMultiValueConverter
{
    public delegate object ConvertDelegate(object[] values, Type targetType, object parameter, CultureInfo culture);
    public delegate object[] ConvertBackDelegate(object value, Type[] targetTypes, object parameter, CultureInfo culture);

    public InlineMultiConverter(ConvertDelegate convert, ConvertBackDelegate? convertBack = null)
    {
        this.convert = convert ?? throw new ArgumentNullException(nameof(convert));
        this.convertBack = convertBack;
    }

    private ConvertDelegate convert { get; }

    private ConvertBackDelegate? convertBack { get; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        => this.convert(values, targetType, parameter, culture);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => this.convertBack != null
            ? this.convertBack(value, targetTypes, parameter, culture)
            : throw new NotImplementedException();
}
