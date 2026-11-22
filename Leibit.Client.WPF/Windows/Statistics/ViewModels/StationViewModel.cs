using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Statistics;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class StationViewModel : ViewModelBase
    {
        #region - Ctor -
        public StationViewModel(StationDelayStatistic statistic)
        {
            StationShort = statistic.Station.ShortSymbol;
            StationName = statistic.Station.Name;
            DelayString = $"+{statistic.DelaySum}";
        }
        #endregion

        #region - Properties -

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
