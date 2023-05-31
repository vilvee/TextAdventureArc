using System;
using System.Windows.Data;
using System.Windows.Media;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool && (bool)value)
        {
            return (Brush)new BrushConverter().ConvertFrom("#58c8f1"); // Return original color when true
        }
        else
        {
            return (Brush)new BrushConverter().ConvertFrom("#808080"); // Return grey when false
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}