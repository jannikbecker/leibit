using Leibit.Core.Scheduling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Leibit.Core.Client.Converter
{
    public class LeibitTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var leibitTime = value as LeibitTime;

            if (leibitTime == null)
                return string.Empty;

            var timeString = leibitTime.ToString();
            var days = new List<string>();

            switch (leibitTime.Day)
            {
                case eDaysOfService.Monday:
                    days.Add("Mo");
                    break;
                case eDaysOfService.Tuesday:
                    days.Add("Di");
                    break;
                case eDaysOfService.Wednesday:
                    days.Add("Mi");
                    break;
                case eDaysOfService.Thursday:
                    days.Add("Do");
                    break;
                case eDaysOfService.Friday:
                    days.Add("Fr");
                    break;
                case eDaysOfService.Saturday:
                    days.Add("Sa");
                    break;
                case eDaysOfService.Sunday:
                    days.Add("So");
                    break;
                default:
                    break;
            }

            if (days.Any())
            {
                var dayString = string.Join("+", days);
                return $"{dayString} {timeString}";
            }
            else
                return timeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
