using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Common;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class CurrentStatisticsViewModel : ChildWindowViewModelBase, IRefreshable
    {
        #region - Needs -
        private StatisticsBLL m_StatisticsBll;
        #endregion

        #region - Ctor -
        public CurrentStatisticsViewModel(Dispatcher dispatcher, Area area)
        {
            m_StatisticsBll = new();

            Dispatcher = dispatcher;
            ESTWList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).ToObservableCollection();
            Total = new TrainTypeStatisticsViewModel();
            PassengerTrains = new TrainTypeStatisticsViewModel();
            FreightTrains = new TrainTypeStatisticsViewModel();

            Total.ShowTrainSchedule += (_, train) => __ShowTrainSchedule(train);
            PassengerTrains.ShowTrainSchedule += (_, train) => __ShowTrainSchedule(train);
            FreightTrains.ShowTrainSchedule += (_, train) => __ShowTrainSchedule(train);

            if (ESTWList.Count == 1)
                SelectedESTW = ESTWList.Single();

        }
        #endregion

        #region - Properties -

        #region [ESTWList]
        public ObservableCollection<ESTW> ESTWList
        {
            get => Get<ObservableCollection<ESTW>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedESTW]
        public ESTW SelectedESTW
        {
            get => Get<ESTW>();
            set
            {
                Set(value);
                Refresh(null);
            }
        }
        #endregion

        #region [CurrentStatistics]
        public CurrentStatistics CurrentStatistics
        {
            get => Get<CurrentStatistics>();
            private set => Set(value);
        }
        #endregion

        #region [Total]
        public TrainTypeStatisticsViewModel Total
        {
            get => Get<TrainTypeStatisticsViewModel>();
            private set => Set(value);
        }
        #endregion

        #region [PassengerTrains]
        public TrainTypeStatisticsViewModel PassengerTrains
        {
            get => Get<TrainTypeStatisticsViewModel>();
            private set => Set(value);
        }
        #endregion

        #region [FreightTrains]
        public TrainTypeStatisticsViewModel FreightTrains
        {
            get => Get<TrainTypeStatisticsViewModel>();
            private set => Set(value);
        }
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area Area)
        {
            if (SelectedESTW == null)
                return;

            var statResult = m_StatisticsBll.GetCurrentStatistics(SelectedESTW);

            if (!statResult.Succeeded)
            {
                MessageBox.Show(statResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentStatistics = statResult.Result;
            Total.Refresh(CurrentStatistics.Total);
            PassengerTrains.Refresh(CurrentStatistics.PassengerTrains);
            FreightTrains.Refresh(CurrentStatistics.FreightTrains);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__ShowTrainSchedule]
        private void __ShowTrainSchedule(TrainInformation train)
        {
            var Window = new TrainScheduleView(train.Train.Number);
            Window.DataContext = new TrainScheduleViewModel(Window.Dispatcher, train.Train, SelectedESTW.Area);
            OnOpenWindow(Window);
        }
        #endregion

        #endregion
    }
}
