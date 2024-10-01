using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public enum eDisplayType
    {
        PlatformDisplay_Small,

        PlatformDisplay_Large,

        DepartureBoard_Small,

        DepartureBoard_Large,

        PassengerInformation,

        Countdown,

        ArrivalBoard_Small,

        ArrivalBoard_Large,
    }

    public class DisplayType
    {
        internal DisplayType(eDisplayType type, string name, DisplayViewModelBase viewModel, bool multiTrack)
        {
            Type = type;
            Name = name;
            ViewModel = viewModel;
            MultiTrack = multiTrack;
        }

        public eDisplayType Type { get; }

        public string Name { get; }

        public DisplayViewModelBase ViewModel { get; }

        public bool MultiTrack { get; }
    }

    public class DisplayTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayType displayType && parameter is eDisplayType requiredType && displayType.Type == requiredType)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
