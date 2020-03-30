using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leibit.Core.Client.Converter
{
    public class VisibilityNullConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsVisible = value != null;

            if (Invert)
                IsVisible = !IsVisible;

            return IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
