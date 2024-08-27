using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MechEngineerSaver.Converters;


public class NullToBool_Converter : ConvertTo<object>
{
    public override object Convert(object value) => value is not null;
    public override object ElseObj => false;
}
public class NullToVisability_Visible_Converter : ConvertTo<object>
{
    public override object Convert(object value) => Visibility.Visible;
    public override object ElseObj => Visibility.Collapsed;
}
public class NullToVisability_Visible_Hidden_Converter : ConvertTo<object>
{
    public override object Convert(object value) => Visibility.Visible;
    public override object ElseObj => Visibility.Hidden;
}
public class NullToVisability_Collapsed_Converter : ConvertTo<object>
{
    public override object Convert(object value) => Visibility.Collapsed;
    public override object ElseObj => Visibility.Visible;
}
public class TrueToVisability_Converter : ConvertTo<bool>
{
    public override object Convert(bool b)
    {
        if (b == true) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}
public class TrueToVisability_Hidden_Converter : ConvertTo<bool>
{
    public override object Convert(bool b)
    {
        if (b == true) return Visibility.Visible;
        else return Visibility.Hidden;
    }
    public override object ElseObj => Visibility.Hidden;
}
public class FalseToVisability_Converter : ConvertTo<bool>
{
    public override object Convert(bool b)
    {
        if (b == false) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}
public class FalseToVisability_Hidden_Converter : ConvertTo<bool>
{
    public override object Convert(bool b)
    {
        if (b == false) return Visibility.Visible;
        else return Visibility.Hidden;
    }
    public override object ElseObj => Visibility.Hidden;
}
public class DoubleToInteger : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (int)Math.Round((double)value, MidpointRounding.ToZero);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}
public class DoubleToWidth : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (double)value;
        if (val > 0) return 60 * (val - Math.Truncate(val));
        return 60 * (-val + Math.Truncate(val));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}
public class DoubleToAlignment : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (double)value > 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}
public class DoubleToThickness : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        new Thickness((int)Math.Abs(Math.Truncate((double)value)) * 2);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}
public class EmptyStringToBool_Converter : ConvertTo<string>
{
    public override object Convert(string value) => !string.IsNullOrWhiteSpace(value);
    public override object ElseObj => false;
}
public class EmptyStringToVisability_Visible_Converter : ConvertTo<string>
{
    public override object Convert(string value)
    {
        if (!string.IsNullOrWhiteSpace(value)) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}
public class EmptyStringToVisability_Visible_Hidden_Converter : ConvertTo<string>
{
    public override object Convert(string value)
    {
        if (!string.IsNullOrWhiteSpace(value)) return Visibility.Visible;
        else return Visibility.Hidden;
    }
    public override object ElseObj => Visibility.Hidden;
}
public class EmptyStringToVisability_Collapsed_Converter : ConvertTo<string>
{
    public override object Convert(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}
public class EmptyStringToVisability_Collapsed_Hidden_Converter : ConvertTo<string>
{
    public override object Convert(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Visibility.Visible;
        else return Visibility.Hidden;
    }
    public override object ElseObj => Visibility.Hidden;
}
public class PositiveIntToVisability_Converter : ConvertTo<int>
{
    public override object Convert(int val)
    {
        if (val > 0) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}
public class AnyIntToVisability_Converter : ConvertTo<int>
{
    public override object Convert(int val)
    {
        if (val > 0 || val < 0) return Visibility.Visible;
        else return Visibility.Collapsed;
    }
    public override object ElseObj => Visibility.Collapsed;
}