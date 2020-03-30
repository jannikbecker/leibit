using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Leibit.Core.Client.Converter
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? IntValue = value as int?;

            if (value == null)
                return SystemColors.WindowFrameColor;

            var Bytes = BitConverter.GetBytes(IntValue.Value);

            if (Bytes.Length == 4)
                return Color.FromArgb(Bytes[3], Bytes[2], Bytes[1], Bytes[0]);

            return SystemColors.WindowFrameColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Color))
                return null;

            Color Col = (Color)value;

            var Bytes = new byte[] { Col.B, Col.G, Col.R, Col.A };
            return BitConverter.ToInt32(Bytes, 0);
        }
    }
}
