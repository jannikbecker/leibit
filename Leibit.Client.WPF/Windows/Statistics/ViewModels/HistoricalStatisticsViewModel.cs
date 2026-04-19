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
using LiveChartsCore.Kernel;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class HistoricalStatisticsViewModel : ChildWindowViewModelBase, IRefreshable
    {
        #region - Needs -
        private StatisticsBLL m_StatisticsBll;
        private PieDataViewModel<int> m_NumberOfEarlyTrains;
        private PieDataViewModel<int> m_NumberOfTrainsOnTime;
        private PieDataViewModel<int> m_NumberOfTrainsWithShortDelay;
        private PieDataViewModel<int> m_NumberOfTrainsWithLongDelay;
        #endregion

        #region - Ctor -
        public HistoricalStatisticsViewModel(Dispatcher dispatcher, Area area)
        {
            m_StatisticsBll = new();
            (App.Current as App).SkinChanged += __SkinChanged;

            Dispatcher = dispatcher;
            ESTWList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).ToObservableCollection();
            ShowTrainWithSmallestDelayScheduleCommand = new CommandHandler(() => __ShowTrainSchedule(HistoricalStatistics?.TrainWithSmallestDelay), true);
            ShowTrainWithGreatestDelayScheduleCommand = new CommandHandler(() => __ShowTrainSchedule(HistoricalStatistics?.TrainWithGreatestDelay), true);

            if (ESTWList.Count == 1)
                SelectedESTW = ESTWList.Single();

            FrameSize = 15;

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

        #region [FrameSize]
        public int FrameSize
        {
            get => Get<int>();
            set => Set(value);
        }
        #endregion

        #region [HistoricalStatistics]
        public HistoricalStatistics HistoricalStatistics
        {
            get => Get<HistoricalStatistics>();
            private set => Set(value);
        }
        #endregion

        #region [ToolTipFormatterFunc]
        public Func<ChartPoint, string> ToolTipFormatterFunc => __GetToolTipLabel;
        #endregion

        #region [DelayStatistics]
        public PieDataViewModel<int>[] DelayStatistics
        {
            get => [m_NumberOfEarlyTrains, m_NumberOfTrainsOnTime, m_NumberOfTrainsWithShortDelay, m_NumberOfTrainsWithLongDelay];
        }
        #endregion

        #region [Frames]
        public string[] Frames
        {
            get => Get<string[]>();
            private set => Set(value);
        }
        #endregion

        #region [NumbersOfTrains]
        public int[] NumbersOfTrains
        {
            get => Get<int[]>();
            private set => Set(value);
        }
        #endregion

        #region [AverageDelays]
        public double[] AverageDelays
        {
            get => Get<double[]>();
            private set => Set(value);
        }
        #endregion

        #region [ForegroundColor]
        public Paint ForegroundColor
        {
            get => new SolidColorPaint((App.Current.TryFindResource("TextForeground") as SolidColorBrush).Color.ToSKColor());
        }
        #endregion

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

        #region - Public methods -

        #region [Refreh]
        public void Refresh(Area Area)
        {
            if (SelectedESTW == null)
                return;

            var statResult = m_StatisticsBll.GetHistoricalStatistics(SelectedESTW, FrameSize);

            if (!statResult.Succeeded)
            {
                MessageBox.Show(statResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HistoricalStatistics = statResult.Result;
            m_NumberOfEarlyTrains.Value = HistoricalStatistics.NumberOfEarlyTrains;
            m_NumberOfTrainsOnTime.Value = HistoricalStatistics.NumberOfTrainsOnTime;
            m_NumberOfTrainsWithShortDelay.Value = HistoricalStatistics.NumberOfTrainsWithShortDelay;
            m_NumberOfTrainsWithLongDelay.Value = HistoricalStatistics.NumberOfTrainsWithLongDelay;

            Frames = HistoricalStatistics.TimeFrames.Select(f => f.StartTime.ToString()).ToArray();
            NumbersOfTrains = HistoricalStatistics.TimeFrames.Select(f => f.NumberOfTrains).ToArray();
            AverageDelays = HistoricalStatistics.TimeFrames.Select(f => f.AverageDelay).ToArray();
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

        #region [__GetToolTipLabel]
        private string __GetToolTipLabel(ChartPoint point)
        {
            var frame = HistoricalStatistics.TimeFrames[point.Index];
            return frame.StartTime.ToString() + " - " + frame.EndTime.ToString();
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
