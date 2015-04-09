// ****************************************************************************
// <copyright file="BooleanToValueConverterBase.cs" company="Pedro Lamas">
// Copyright © Pedro Lamas 2013
// </copyright>
// ****************************************************************************
// <author>Pedro Lamas</author>
// <email>pedrolamas@gmail.com</email>
// <date>26-04-2013</date>
// <project>Cimbalino.Phone.Toolkit</project>
// <web>http://www.pedrolamas.com</web>
// <license>
// See license.txt in this solution or http://www.pedrolamas.com/license_MIT.txt
// </license>
// ****************************************************************************

using System;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Shell;

namespace Emby.WindowsPhone.Converters
{
    /// <summary>
    /// An <see cref="IValueConverter"/> abstract implementation which converts a <see cref="bool"/> value to a value of the specified type.
    /// </summary>
    /// <typeparam name="T">The converter type.</typeparam>
    public abstract class BooleanToValueConverterBase<T> : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register("TrueValue", typeof (T), typeof (BooleanToValueConverterBase<T>), new PropertyMetadata(default(T)));

        public T TrueValue
        {
            get { return (T) GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register("FalseValue", typeof (T), typeof (BooleanToValueConverterBase<T>), new PropertyMetadata(default(T)));

        public T FalseValue
        {
            get { return (T) GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        /// <summary>
        /// Converts a <see cref="bool"/> value to a a value of the specified type.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts a value from the specified type to a <see cref="bool"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
    }

    public class BooleanToStringConverter : BooleanToValueConverterBase<string>{}

    public class BooleanToAppBarModeConverter : BooleanToValueConverterBase<ApplicationBarMode> { }
}