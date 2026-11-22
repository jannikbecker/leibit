using System;
using System.Globalization;
using System.Windows.Data;

namespace Leibit.Client.WPF.Windows.Statistics.Views
{
    public class DelayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int delay)
            {
                if (delay < 0)
                    return $"{delay} min";
                else
                    return $"+{delay} min";
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
