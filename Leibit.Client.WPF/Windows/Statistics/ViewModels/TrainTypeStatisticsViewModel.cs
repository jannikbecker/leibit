using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp.Views.WPF;
using System;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class TrainTypeStatisticsViewModel : ViewModelBase
    {
        #region - Needs -
        private PieDataViewModel<int> m_NumberOfEarlyTrains;
        private PieDataViewModel<int> m_NumberOfTrainsOnTime;
        private PieDataViewModel<int> m_NumberOfTrainsWithShortDelay;
        private PieDataViewModel<int> m_NumberOfTrainsWithLongDelay;
        #endregion

        #region - Ctor -
        public TrainTypeStatisticsViewModel()
        {
            (App.Current as App).SkinChanged += __SkinChanged;

            m_NumberOfEarlyTrains = new PieDataViewModel<int>("< -5 min", (App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime = new PieDataViewModel<int>("Pünktlich", (App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay = new PieDataViewModel<int>("> 5 min", (App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay = new PieDataViewModel<int>("> 10 min", (App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);

            ShowTrainWithGreatestDelayScheduleCommand = new CommandHandler(() => ShowTrainSchedule?.Invoke(this, CurrentStatistics?.TrainWithGreatestDelay), true);
        }
        #endregion

        #region - Events -

        public event EventHandler<TrainInformation> ShowTrainSchedule;

        #endregion

        #region - Properties -

        #region [CurrentStatistics]
        public TrainTypeStatistics CurrentStatistics
        {
            get => Get<TrainTypeStatistics>();
            private set => Set(value);
        }
        #endregion

        #region [HasDelayedTrains]
        public bool HasDelayedTrains
        {
            get => CurrentStatistics.TrainWithGreatestDelay != null;
        }
        #endregion

        #region [OnTimeRateString]
        public string OnTimeRateString
        {
            get
            {
                if (CurrentStatistics?.OnTimeRate == null)
                    return "- %";

                return $"{CurrentStatistics.OnTimeRate} %";
            }
        }
        #endregion

        #region [DelayStatistics]
        public PieDataViewModel<int>[] DelayStatistics
        {
            get => [m_NumberOfEarlyTrains, m_NumberOfTrainsOnTime, m_NumberOfTrainsWithShortDelay, m_NumberOfTrainsWithLongDelay];
        }
        #endregion

        #region [ForegroundColor]
        public Paint ForegroundColor
        {
            get => new SolidColorPaint((App.Current.TryFindResource("TextForeground") as SolidColorBrush).Color.ToSKColor());
        }
        #endregion

        #region [ShowTrainWithGreatestDelayScheduleCommand]
        public CommandHandler ShowTrainWithGreatestDelayScheduleCommand { get; }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(TrainTypeStatistics statistics)
        {
            CurrentStatistics = statistics;
            OnPropertyChanged(nameof(CurrentStatistics));
            OnPropertyChanged(nameof(OnTimeRateString));
            OnPropertyChanged(nameof(HasDelayedTrains));
            m_NumberOfEarlyTrains.Value = statistics.NumberOfEarlyTrains;
            m_NumberOfTrainsOnTime.Value = statistics.NumberOfTrainsOnTime;
            m_NumberOfTrainsWithShortDelay.Value = statistics.NumberOfTrainsWithShortDelay;
            m_NumberOfTrainsWithLongDelay.Value = statistics.NumberOfTrainsWithLongDelay;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__SkinChanged]
        private void __SkinChanged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(ForegroundColor));
            m_NumberOfEarlyTrains.SetFillColor((App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime.SetFillColor((App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay.SetFillColor((App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay.SetFillColor((App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);
        }
        #endregion

        #endregion
    }
}
