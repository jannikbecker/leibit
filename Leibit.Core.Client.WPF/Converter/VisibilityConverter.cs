using System;
using System.Windows;
using System.Windows.Data;

namespace Leibit.Core.Client.Converter
{
    public class VisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsVisible = false;

            if (value != null)
                Boolean.TryParse(value.ToString(), out IsVisible);

            if (Invert)
                IsVisible = !IsVisible;

            return IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility Visibility = Visibility.Collapsed;

            if (value != null)
                Enum.TryParse(value.ToString(), out Visibility);

            var Result = Visibility == Visibility.Visible ? true : false;

            if (Invert)
                return !Result;

            return Result;
        }
    }
}
