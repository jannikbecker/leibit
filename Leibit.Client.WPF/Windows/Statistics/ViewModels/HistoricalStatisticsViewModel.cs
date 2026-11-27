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
using System.Collections.Generic;
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
        #endregion

        #region - Ctor -
        public HistoricalStatisticsViewModel(Dispatcher dispatcher, Area area)
        {
            m_StatisticsBll = new();

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
            ShowStationDelaysCommand = new CommandHandler(() => __ShowStationDelays(SelectedStation.CurrentStation), true);
            ShowTrainScheduleFromDelayCommand = new CommandHandler(() => __ShowTrainSchedule(SelectedDelayDetail.Train), true);
            CloseStationDetailsCommand = new CommandHandler(__CloseStationDetails, true);
            DelayStatistics = new();
            TrainsWithGreatestDelays = new();
            StationsWithGreatestDelays = new();
            StationDelayDetails = new();

            if (ESTWList.Count == 1)
                SelectedESTW = ESTWList.Single();

            (App.Current as App).SkinChanged += __SkinChanged;
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

        #region [DelayStatistics]
        public DelayStatisticsViewModel DelayStatistics
        {
            get => Get<DelayStatisticsViewModel>();
            private set => Set(value);
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

        #region [NumberOfTrainsStroke]
        public Paint NumberOfTrainsStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("StatisticsNumberOfTrainsLine") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [DelaysStroke]
        public Paint DelaysStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("StatisticsDelaySumLine") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [OnTimeRatesStroke]
        public Paint OnTimeRatesStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("StatisticsOnTimeRateLine") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [ToolTipForegroundStroke]
        public Paint ToolTipForegroundStroke
        {
            get => new SolidColorPaint((App.Current.TryFindResource("ToolTipForeground") as SolidColorBrush).Color.ToSKColor(), 2);
        }
        #endregion

        #region [ToolTipBackgroundStroke]
        public Paint ToolTipBackgroundStroke
        {
            get
            {
                var color = (App.Current.TryFindResource("ToolTipBackground") as SolidColorBrush).Color;
                color.A = 220;
                return new SolidColorPaint(color.ToSKColor(), 2);
            }
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

        #region [SelectedStation]
        public StationViewModel SelectedStation
        {
            get => Get<StationViewModel>();
            set => Set(value);
        }
        #endregion

        #region [IsStationDetailVisible]
        public bool IsStationDetailVisible
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [StationDelayDetails]
        public ObservableCollection<DelayDetailViewModel> StationDelayDetails
        {
            get => Get<ObservableCollection<DelayDetailViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedDelayDetail]
        public DelayDetailViewModel SelectedDelayDetail
        {
            get => Get<DelayDetailViewModel>();
            set => Set(value);
        }
        #endregion

        #region [ShowTrainScheduleCommand]
        public CommandHandler ShowTrainScheduleCommand { get; }
        #endregion

        #region [ShowStationDelaysCommand]
        public CommandHandler ShowStationDelaysCommand { get; }
        #endregion

        #region [ShowTrainScheduleFromDelayCommand]
        public CommandHandler ShowTrainScheduleFromDelayCommand { get; }
        #endregion

        #region [CloseStationDetailsCommand]
        public CommandHandler CloseStationDetailsCommand { get; }
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
            DelayStatistics.Refresh(HistoricalStatistics.DelayStatistics);

            Frames = HistoricalStatistics.TimeFrames.Select(f => f.StartTime.ToString()).ToArray();
            NumbersOfTrains = HistoricalStatistics.TimeFrames.Select(f => f.NumberOfTrains).ToArray();
            Delays = HistoricalStatistics.TimeFrames.Select(f => f.DelaySum).ToArray();
            OnTimeRates = HistoricalStatistics.TimeFrames.Select(f => f.OnTimeRate).ToArray();

            __SyncCollections(HistoricalStatistics.TrainsWithGreatestDelays.Select(__GetDelayedTrainVM), TrainsWithGreatestDelays);
            __SyncCollections(HistoricalStatistics.StationsWithGreatestDelays.Select(__GetStationVM), StationsWithGreatestDelays);

            if (IsStationDetailVisible)
                __ShowStationDelays(SelectedStation.CurrentStation);
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

        #region [__ShowStationDelays]
        private void __ShowStationDelays(Station station)
        {
            var statResult = m_StatisticsBll.GetDelaysForStation(station);

            if (!statResult.Succeeded)
            {
                MessageBox.Show(statResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            __SyncCollections(statResult.Result.Select(__GetDelayDetailVM), StationDelayDetails);
            IsStationDetailVisible = true;
        }
        #endregion

        #region [__CloseStationDetails]
        private void __CloseStationDetails()
        {
            IsStationDetailVisible = false;
        }
        #endregion

        #region [__GetDelayedTrainVM]
        private DelayedTrainViewModel __GetDelayedTrainVM(TrainDelayStatistic statistic)
        {
            var vm = TrainsWithGreatestDelays.FirstOrDefault(vm => vm.CurrentTrain == statistic.Train);

            if (vm == null)
                vm = new DelayedTrainViewModel(statistic);

            vm.Refresh(statistic);
            return vm;
        }
        #endregion

        #region [__GetStationVM]
        private StationViewModel __GetStationVM(StationDelayStatistic statistic)
        {
            var vm = StationsWithGreatestDelays.FirstOrDefault(vm => vm.CurrentStation == statistic.Station);

            if (vm == null)
                vm = new StationViewModel(statistic);

            vm.Refresh(statistic);
            return vm;
        }
        #endregion

        #region [__GetDelayDetailVM]
        private DelayDetailViewModel __GetDelayDetailVM(DelayInfo delay)
        {
            var vm = StationDelayDetails.FirstOrDefault(vm => vm.CurrentDelayInfo == delay);

            if (vm == null)
                vm = new DelayDetailViewModel(delay);

            vm.Refresh(delay);
            return vm;
        }
        #endregion

        #region [__SyncCollections]
        private void __SyncCollections<T>(IEnumerable<T> source, ObservableCollection<T> target)
        {
            Dispatcher.Invoke(() =>
            {
                // 1. Delete elements that no longer exist
                target.Except(source).ToList().ForEach(x => target.Remove(x));

                // 2. Insert new elements & ensure sorting is correct.
                for (int i = 0; i < source.Count(); i++)
                {
                    var currentItem = source.ElementAt(i);
                    int existingIndex = target.IndexOf(currentItem);

                    if (existingIndex == -1)
                    {
                        target.Insert(i, currentItem);
                    }
                    else if (existingIndex != i)
                    {
                        target.Move(existingIndex, i);
                    }
                }
            });
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
            OnPropertyChanged(nameof(NumberOfTrainsStroke));
            OnPropertyChanged(nameof(DelaysStroke));
            OnPropertyChanged(nameof(OnTimeRatesStroke));
            OnPropertyChanged(nameof(ToolTipForegroundStroke));
            OnPropertyChanged(nameof(ToolTipBackgroundStroke));
        }
        #endregion

        #endregion
    }
}
