using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp.Views.WPF;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class CurrentStatisticsViewModel : ChildWindowViewModelBase, IRefreshable
    {
        #region - Needs -
        private StatisticsBLL m_StatisticsBll;
        private PieDataViewModel<int> m_NumberOfEarlyTrains;
        private PieDataViewModel<int> m_NumberOfTrainsOnTime;
        private PieDataViewModel<int> m_NumberOfTrainsWithShortDelay;
        private PieDataViewModel<int> m_NumberOfTrainsWithLongDelay;
        #endregion

        #region - Ctor -
        public CurrentStatisticsViewModel(Dispatcher dispatcher, Area area)
        {
            m_StatisticsBll = new();
            (App.Current as App).SkinChanged += __SkinChanged;

            Dispatcher = dispatcher;
            ESTWList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).ToObservableCollection();
            ShowTrainWithSmallestDelayScheduleCommand = new CommandHandler(() => __ShowTrainSchedule(CurrentStatistics?.TrainWithSmallestDelay), true);
            ShowTrainWithGreatestDelayScheduleCommand = new CommandHandler(() => __ShowTrainSchedule(CurrentStatistics?.TrainWithGreatestDelay), true);

            if (ESTWList.Count == 1)
                SelectedESTW = ESTWList.Single();

            m_NumberOfEarlyTrains = new PieDataViewModel<int>("< -5 min", (App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime = new PieDataViewModel<int>("Pünktlich", (App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay = new PieDataViewModel<int>("> 5 min", (App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay = new PieDataViewModel<int>("> 10 min", (App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);
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
            set => Set(value);
        }
        #endregion

        #region [CurrentStatistics]
        public CurrentStatistics CurrentStatistics
        {
            get => Get<CurrentStatistics>();
            private set => Set(value);
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

        #region [ShowTrainWithSmallestDelayScheduleCommand]
        public CommandHandler ShowTrainWithSmallestDelayScheduleCommand { get; }
        #endregion

        #region [ShowTrainWithGreatestDelayScheduleCommand]
        public CommandHandler ShowTrainWithGreatestDelayScheduleCommand { get; }
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

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
            m_NumberOfEarlyTrains.Value = CurrentStatistics.NumberOfEarlyTrains;
            m_NumberOfTrainsOnTime.Value = CurrentStatistics.NumberOfTrainsOnTime;
            m_NumberOfTrainsWithShortDelay.Value = CurrentStatistics.NumberOfTrainsWithShortDelay;
            m_NumberOfTrainsWithLongDelay.Value = CurrentStatistics.NumberOfTrainsWithLongDelay;
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
