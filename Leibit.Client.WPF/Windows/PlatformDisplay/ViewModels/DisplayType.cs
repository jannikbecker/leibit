namespace Leibit.Client.WPF.Windows.PlatformDisplay.ViewModels
{
    public enum eDisplayType
    {
        PlatformDisplay_Small,

        PlatformDisplay_Large,

        DepartureBoard_Small,

        DepartureBoard_Large,

        PassengerInformation,
    }

    public class DisplayType
    {
        public DisplayType(eDisplayType type, string name)
        {
            Type = type;
            Name = name;
        }

        public eDisplayType Type { get; }

        public string Name { get; }
    }
}
