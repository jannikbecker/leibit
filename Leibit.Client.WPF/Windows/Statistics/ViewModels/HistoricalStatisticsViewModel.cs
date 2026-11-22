using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities;
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
        public class StatisticType
        {
            public string Name { get; set; }
            public eHistoricalStatisticsType Type { get; set; }
        }

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

            m_NumberOfEarlyTrains = new PieDataViewModel<int>("< -5 min", (App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color);
            m_NumberOfTrainsOnTime = new PieDataViewModel<int>("Pünktlich", (App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color);
            m_NumberOfTrainsWithShortDelay = new PieDataViewModel<int>("> 5 min", (App.Current.TryFindResource("DelayYellow") as SolidColorBrush).Color);
            m_NumberOfTrainsWithLongDelay = new PieDataViewModel<int>("> 10 min", (App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color);

            Dispatcher = dispatcher;
            FrameSize = 15;
            ShowNumbersOfTrains = true;
            ShowDelays = true;
            ShowOnTimeRates = true;
            StatisticTypes = [
                new StatisticType { Name = "Alle Züge", Type = eHistoricalStatisticsType.All},
                new StatisticType { Name = "Nur Reisezüge", Type = eHistoricalStatisticsType.OnlyPassengerTrains},
                new StatisticType { Name = "Nur Güterzüge", Type = eHistoricalStatisticsType.OnlyFreightFrains},
            ];
            SelectedStatisticType = StatisticTypes[0];
            ESTWList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).ToObservableCollection();
            ShowTrainScheduleCommand = new CommandHandler(() => __ShowTrainSchedule(SelectedTrain.CurrentTrain), true);

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

        #region [StatisticTypes]
        public ObservableCollection<StatisticType> StatisticTypes
        {
            get => Get<ObservableCollection<StatisticType>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedStatisticType]
        public StatisticType SelectedStatisticType
        {
            get => Get<StatisticType>();
            set
            {
                Set(value);
                Refresh(null);
            }
        }
        #endregion

        #region [FrameSize]
        public int FrameSize
        {
            get => Get<int>();
            set
            {
                Set(value);
                Refresh(null);
            }
        }
        #endregion

        #region [ShowNumbersOfTrains]
        public bool ShowNumbersOfTrains
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [ShowDelays]
        public bool ShowDelays
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [ShowOnTimeRates]
        public bool ShowOnTimeRates
        {
            get => Get<bool>();
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

        #region [OnTimeRateString]
        public string OnTimeRateString
        {
            get
            {
                if (HistoricalStatistics?.OnTimeRate == null)
                    return "- %";

                return $"{HistoricalStatistics.OnTimeRate} %";
            }
        }
        #endregion

        #region [ToolTipFormatterFunc]
        public Func<ChartPoint, string> ToolTipFormatterFunc => __GetToolTipLabel_Header;
        #endregion

        #region [ToolTipFormatterDelay]
        public Func<ChartPoint, string> ToolTipFormatterDelay => __GetToolTipLabel_Delay;
        #endregion

        #region [ToolTipFormatterOnTimeRate]
        public Func<ChartPoint, string> ToolTipFormatterOnTimeRate => __GetToolTipLabel_OnTimeRate;
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

        #region [Delays]
        public double[] Delays
        {
            get => Get<double[]>();
            private set => Set(value);
        }
        #endregion

        #region [OnTimeRates]
        public int[] OnTimeRates
        {
            get => Get<int[]>();
            private set => Set(value);
        }
        #endregion

        #region [ForegroundColor]
        public Paint ForegroundColor
        {
            get => new SolidColorPaint((App.Current.TryFindResource("TextForeground") as SolidColorBrush).Color.ToSKColor());
        }
        #endregion

        #region [NumberOfTrainsStroke]
        public Paint NumberOfTrainsStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("DelayBlue") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [DelaysStroke]
        public Paint DelaysStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("DelayRed") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [OnTimeRatesStroke]
        public Paint OnTimeRatesStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("ReadyColor") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [TrainsWithGreatestDelays]
        public ObservableCollection<DelayedTrainViewModel> TrainsWithGreatestDelays
        {
            get => Get<ObservableCollection<DelayedTrainViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [StationsWithGreatestDelays]
        public ObservableCollection<StationViewModel> StationsWithGreatestDelays
        {
            get => Get<ObservableCollection<StationViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedTrain]
        public DelayedTrainViewModel SelectedTrain
        {
            get => Get<DelayedTrainViewModel>();
            set => Set(value);
        }
        #endregion

        #region [ShowTrainScheduleCommand]
        public CommandHandler ShowTrainScheduleCommand { get; }
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refreh]
        public void Refresh(Area Area)
        {
            if (SelectedESTW == null || SelectedStatisticType == null)
                return;

            var statResult = m_StatisticsBll.GetHistoricalStatistics(SelectedESTW, FrameSize, SelectedStatisticType.Type);

            if (!statResult.Succeeded)
            {
                MessageBox.Show(statResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HistoricalStatistics = statResult.Result;
            OnPropertyChanged(nameof(HistoricalStatistics));
            OnPropertyChanged(nameof(OnTimeRateString));

            m_NumberOfEarlyTrains.Value = HistoricalStatistics.NumberOfEarlyTrains;
            m_NumberOfTrainsOnTime.Value = HistoricalStatistics.NumberOfTrainsOnTime;
            m_NumberOfTrainsWithShortDelay.Value = HistoricalStatistics.NumberOfTrainsWithShortDelay;
            m_NumberOfTrainsWithLongDelay.Value = HistoricalStatistics.NumberOfTrainsWithLongDelay;

            Frames = HistoricalStatistics.TimeFrames.Select(f => f.StartTime.ToString()).ToArray();
            NumbersOfTrains = HistoricalStatistics.TimeFrames.Select(f => f.NumberOfTrains).ToArray();
            Delays = HistoricalStatistics.TimeFrames.Select(f => f.DelaySum).ToArray();
            OnTimeRates = HistoricalStatistics.TimeFrames.Select(f => f.OnTimeRate).ToArray();

            TrainsWithGreatestDelays = HistoricalStatistics.TrainsWithGreatestDelays.Select(t => new DelayedTrainViewModel(t)).ToObservableCollection();
            StationsWithGreatestDelays = HistoricalStatistics.StationsWithGreatestDelays.Select(s => new StationViewModel(s)).ToObservableCollection();
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

        #region [__GetToolTipLabel_Header]
        private string __GetToolTipLabel_Header(ChartPoint point)
        {
            var frame = HistoricalStatistics.TimeFrames[point.Index];
            return frame.StartTime.ToString() + " - " + frame.EndTime.ToString();
        }
        #endregion

        #region [__GetToolTipLabel_Delay]
        private string __GetToolTipLabel_Delay(ChartPoint point)
        {
            var delay = Delays[point.Index];
            return $"+{delay} min";
        }
        #endregion

        #region [__GetToolTipLabel_OnTimeRate]
        private string __GetToolTipLabel_OnTimeRate(ChartPoint point)
        {
            var onTimeRate = OnTimeRates[point.Index];
            return $"{onTimeRate}%";
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
