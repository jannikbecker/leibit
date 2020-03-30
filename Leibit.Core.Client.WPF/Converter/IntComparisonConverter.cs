using System;
using System.Windows.Data;

namespace Leibit.Core.Client.Converter
{
    public class IntComparisonConverter : IValueConverter
    {
        public eComparisonType ComparisonType { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            int Value;
            int Reference;

            if (Int32.TryParse(value.ToString(), out Value) && Int32.TryParse(parameter.ToString(), out Reference))
            {
                switch (ComparisonType)
                {
                    case eComparisonType.Smaller:
                        return Value < Reference;

                    case eComparisonType.SmallerOrEqual:
                        return Value <= Reference;

                    case eComparisonType.Greater:
                        return Value > Reference;

                    case eComparisonType.GreaterOrEqual:
                        return Value >= Reference;

                    default:
                        return Value == Reference;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum eComparisonType
    {
        Equal = 0,
        Smaller = 1,
        SmallerOrEqual = 2,
        Greater = 3,
        GreaterOrEqual = 4,
    }
}
