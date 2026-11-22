using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Statistics;
using System;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class DelayStatisticsViewModel : ViewModelBase
    {
        #region - Needs -
        private SeriesViewModel<int> m_NumberOfEarlyTrains;
        private SeriesViewModel<int> m_NumberOfTrainsOnTime;
        private SeriesViewModel<int> m_NumberOfTrainsWithShortDelay;
        private SeriesViewModel<int> m_NumberOfTrainsWithLongDelay;
        #endregion

        #region - Ctor -
        public DelayStatisticsViewModel()
        {
            m_NumberOfEarlyTrains = new SeriesViewModel<int>((App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime = new SeriesViewModel<int>((App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay = new SeriesViewModel<int>((App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay = new SeriesViewModel<int>((App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);
            (App.Current as App).SkinChanged += __SkinChanged;
        }
        #endregion

        #region - Properties -

        #region [DelayStatistics]
        public SeriesViewModel<int>[] DelayStatistics
        {
            get => [m_NumberOfEarlyTrains, m_NumberOfTrainsOnTime, m_NumberOfTrainsWithShortDelay, m_NumberOfTrainsWithLongDelay];
        }
        #endregion

        #region [OverallRateString]
        public string OverallRateString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [NumberOfTrains]
        public int NumberOfTrains
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [NumberOfEarlyTrains]
        public int NumberOfEarlyTrains
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [EarlyTrainsRateString]
        public string EarlyTrainsRateString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [NumberOfTrainsOnTime]
        public int NumberOfTrainsOnTime
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [TrainsOnTimeRateString]
        public string TrainsOnTimeRateString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [NumberOfTrainsWithShortDelay]
        public int NumberOfTrainsWithShortDelay
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [TrainsWithShortDelayRateString]
        public string TrainsWithShortDelayRateString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [NumberOfTrainsWithLongDelay]
        public int NumberOfTrainsWithLongDelay
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [TrainsWithLongDelayRateString]
        public string TrainsWithLongDelayRateString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(DelayStatistics statistics)
        {
            m_NumberOfEarlyTrains.Value = statistics.NumberOfEarlyTrains;
            m_NumberOfTrainsOnTime.Value = statistics.NumberOfTrainsOnTime;
            m_NumberOfTrainsWithShortDelay.Value = statistics.NumberOfTrainsWithShortDelay;
            m_NumberOfTrainsWithLongDelay.Value = statistics.NumberOfTrainsWithLongDelay;

            NumberOfEarlyTrains = statistics.NumberOfEarlyTrains;
            NumberOfTrainsOnTime = statistics.NumberOfTrainsOnTime;
            NumberOfTrainsWithShortDelay = statistics.NumberOfTrainsWithShortDelay;
            NumberOfTrainsWithLongDelay = statistics.NumberOfTrainsWithLongDelay;

            NumberOfTrains = statistics.NumberOfEarlyTrains + statistics.NumberOfTrainsOnTime + statistics.NumberOfTrainsWithShortDelay + statistics.NumberOfTrainsWithLongDelay;

            if (NumberOfTrains > 0)
            {
                var onTimeRate = 100 * (statistics.NumberOfEarlyTrains + statistics.NumberOfTrainsOnTime) / NumberOfTrains;
                OverallRateString = $"{onTimeRate} %";

                var earlyTrainsRate = Math.Round(100.0 * (statistics.NumberOfEarlyTrains) / NumberOfTrains, 2);
                EarlyTrainsRateString = $"{earlyTrainsRate} %";

                var trainsOnTimeRate = Math.Round(100.0 * (statistics.NumberOfTrainsOnTime) / NumberOfTrains, 2);
                TrainsOnTimeRateString = $"{trainsOnTimeRate} %";

                var trainsWithShortDelayRate = Math.Round(100.0 * (statistics.NumberOfTrainsWithShortDelay) / NumberOfTrains, 2);
                TrainsWithShortDelayRateString = $"{trainsWithShortDelayRate} %";

                var trainsWithLongDelayRate = Math.Round(100.0 * (statistics.NumberOfTrainsWithLongDelay) / NumberOfTrains, 2);
                TrainsWithLongDelayRateString = $"{trainsWithLongDelayRate} %";
            }
            else
            {
                OverallRateString = "- %";
                EarlyTrainsRateString = "- %";
                TrainsOnTimeRateString = "- %";
                TrainsWithShortDelayRateString = "- %";
                TrainsWithLongDelayRateString = "- %";
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__SkinChanged]
        private void __SkinChanged(object sender, System.EventArgs e)
        {
            m_NumberOfEarlyTrains.SetFillColor((App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime.SetFillColor((App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay.SetFillColor((App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay.SetFillColor((App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);
        }
        #endregion

        #endregion
    }
}
