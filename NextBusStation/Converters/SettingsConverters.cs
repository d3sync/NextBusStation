using System.Globalization;

namespace NextBusStation.Converters;

public class DataTypeVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dataType && parameter is string allowedTypes)
        {
            var types = allowedTypes.Split('|');
            return types.Contains(dataType);
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return bool.TryParse(str, out var result) && result;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue.ToString().ToLower();
        }
        return "false";
    }
}

public class StringToDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && double.TryParse(str, out var result))
        {
            return result;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return doubleValue.ToString();
        }
        return "0";
    }
}

public class StringToTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && TimeSpan.TryParse(str, out var result))
        {
            return result;
        }
        return TimeSpan.Zero;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeValue)
        {
            return $"{timeValue.Hours:D2}:{timeValue.Minutes:D2}";
        }
        return "00:00";
    }
}

public class KeyboardTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dataType)
        {
            return dataType switch
            {
                "int" or "double" => Keyboard.Numeric,
                _ => Keyboard.Default
            };
        }
        return Keyboard.Default;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
