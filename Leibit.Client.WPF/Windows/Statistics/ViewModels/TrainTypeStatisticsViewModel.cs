using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using System;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class TrainTypeStatisticsViewModel : ViewModelBase
    {
        #region - Ctor -
        public TrainTypeStatisticsViewModel()
        {
            ShowTrainWithGreatestDelayScheduleCommand = new CommandHandler(() => ShowTrainSchedule?.Invoke(this, CurrentStatistics?.TrainWithGreatestDelay), true);
            DelayStatistics = new();
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

        #region [DelayStatistics]
        public DelayStatisticsViewModel DelayStatistics
        {
            get => Get<DelayStatisticsViewModel>();
            private set => Set(value);
        }
        #endregion

        #region [HasDelayedTrains]
        public bool HasDelayedTrains
        {
            get => CurrentStatistics.TrainWithGreatestDelay != null;
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
            OnPropertyChanged(nameof(HasDelayedTrains));
            DelayStatistics.Refresh(statistics.DelayStatistics);
        }
        #endregion

        #endregion
    }
}
