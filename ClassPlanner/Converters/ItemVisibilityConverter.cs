using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace ClassPlanner.Converters;

public partial class ItemToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (parameter is string param && string.Compare(param, "invert") == 0)
        {
            // invert Convert
            object result = Convert(value, targetType, null, language);
            if (result is bool r)
                return !r;
            if (result is Visibility v)
                return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        // Check the property type, sometimes is IsEnabled (return a bool), sometimes is Visibility (return Windows.UI.Xaml.Visibility)
        // Using a converter, UIElement.Visibility disables the cast from bool to Windows.UI.Xaml.Visibility
        object trueValue;
        object falseValue;
        if (targetType == typeof(Visibility))
        {
            trueValue = Visibility.Visible;
            falseValue = Visibility.Collapsed;
        }
        else
        {
            trueValue = true;
            falseValue = false;
        }

        if (value is null)
        {
            return falseValue;
        }
        Type type = value?.GetType()!;

        if (value is string s)
            return string.IsNullOrWhiteSpace(s) ? falseValue : trueValue;

        if (type.IsValueType)
        {
            object defaultValue = Activator.CreateInstance(type)!;
            return value!.Equals(defaultValue) ? falseValue : trueValue;
        }
        return value == default ? falseValue : trueValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
