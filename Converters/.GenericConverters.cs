using System.Globalization;
using System.Windows.Data;

namespace MechEngineerSaver.Converters;

public abstract class ConvertTo<V> : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val)
            return Convert(val);
        else return ElseObj;
    }
    public abstract object Convert(V value);
    public abstract object ElseObj { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertBack(value)!;
    public virtual V ConvertBack(object? value) { throw new Exception(); }
}
public abstract class ConvertTo<V, P> : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val && parameter is P param)
            return Convert(val, param);
        else return ElseObj;
    }
    public abstract object Convert(V value, P parameter);
    public abstract object ElseObj { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}
public abstract class ConvertToNullParam<V, P> : IValueConverter where P : class
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val)
        {
            if (parameter is P param)
                return Convert(val, param);
            return Convert(val, null!);
        }
        else return ElseObj;
    }
    public abstract object Convert(V value, P parameter);
    public abstract object ElseObj { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new Exception();
}

public abstract class ConvertToBack<V, B> : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val)
            return ConvertTo(val)!;
        else return ElseObjTo!;
    }
    public abstract object ConvertTo(V value);
    public abstract object ElseObjTo { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is B val)
            return ConvertBack(val)!;
        else return ElseObjBack!;
    }
    public abstract object ConvertBack(B value);
    public abstract object ElseObjBack { get; }
}
public abstract class ConvertToBack<V, B, P> : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val && parameter is P param)
            return ConvertTo(val, param)!;
        else return ElseObjTo!;
    }
    public abstract object ConvertTo(V value, P parameter);
    public abstract object ElseObjTo { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is B val && parameter is P param)
            return ConvertBack(val, param)!;
        else return ElseObjBack!;
    }
    public abstract object ConvertBack(B value, P parameter);
    public abstract object ElseObjBack { get; }
}
public abstract class ConvertToBackNullParam<V, B, P> : IValueConverter where P : class
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is V val)
        {
            if (parameter is P param)
                return ConvertTo(val, param)!;
            return ConvertTo(val, null!)!;
        }
        else return ElseObjTo!;
    }
    public abstract object ConvertTo(V value, P parameter);
    public abstract object ElseObjTo { get; }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is B val)
        {
            if (parameter is P param)
                return ConvertBack(val, param)!;
            return ConvertBack(val, null!)!;
        }
        else return ElseObjBack!;
    }
    public abstract object ConvertBack(B value, P parameter);
    public abstract object ElseObjBack { get; }
}