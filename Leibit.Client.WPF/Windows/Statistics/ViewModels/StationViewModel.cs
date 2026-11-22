using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Common;
using Leibit.Entities.Statistics;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class StationViewModel : ViewModelBase
    {
        #region - Ctor -
        public StationViewModel(StationDelayStatistic statistic)
        {
            CurrentStation = statistic.Station;
            StationShort = statistic.Station.ShortSymbol;
            StationName = statistic.Station.Name;
            DelayString = $"+{statistic.DelaySum}";
        }
        #endregion

        #region - Properties -

        #region [CurrentStation]
        public Station CurrentStation
        {
            get => Get<Station>();
            private set => Set(value);
        }
        #endregion

        #region [StationShort]
        public string StationShort
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [StationName]
        public string StationName
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [DelayString]
        public string DelayString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #endregion
    }
}
