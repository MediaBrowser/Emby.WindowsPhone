using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Emby.WindowsPhone.Model;

namespace Emby.WindowsPhone.Converters
{
    public class EnumToBooleanConverter<TEnum> : IValueConverter where TEnum : struct
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            // Convert parameter from string to enum if needed.
            TEnum enumValue;
            if (parameter is string &&
                Enum.TryParse((string)parameter, true, out enumValue))
            {
                parameter = enumValue;
            }
            // Return true if value matches parameter.
            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            // If value is true, then return the enum value corresponding to parameter.
            if (Equals(value, true))
            {
                // Convert parameter from string to enum if needed.
                TEnum enumValue;
                if (parameter is string &&
                    Enum.TryParse((string)parameter, true, out enumValue))
                {
                    parameter = enumValue;
                }
                return parameter;
            }
            // Otherwise, return UnsetValue, which is ignored by bindings.
            return DependencyProperty.UnsetValue;
        }
    }

    public class LockScreenTypeConverter : EnumToBooleanConverter<LockScreenType>
    {
    }
}
