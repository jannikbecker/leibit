using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.TrainProgressInformation.ViewModels
{
    public class TrainProgressInformationViewModel : ChildWindowViewModelBase, IRefreshable, ILayoutSavable
    {

        #region - Needs -
        private CommandHandler m_DoubleClickCommand;
        #endregion

        #region - Ctor -
        public TrainProgressInformationViewModel(Dispatcher Dispatcher)
        {
            this.Dispatcher = Dispatcher;
            Trains = new ObservableCollection<TrainStationViewModel>();
            m_DoubleClickCommand = new CommandHandler(__RowDoubleClick, true);
        }
        #endregion

        #region - Properties -

        #region [Trains]
        public ObservableCollection<TrainStationViewModel> Trains
        {
            get;
            private set;
        }
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
        #endregion

        #region [DoubleClickCommand]
        public ICommand DoubleClickCommand
        {
            get
            {
                return m_DoubleClickCommand;
            }
        }
        #endregion

        #region [SaveGridLayout]
        public bool SaveGridLayout
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [SelectedItem]
        public TrainStationViewModel SelectedItem
        {
            get
            {
                return Get<TrainStationViewModel>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [SelectedColumn]
        public string SelectedColumn
        {
            private get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area Area)
        {
            var CurrentTrains = new List<TrainStationViewModel>();

            foreach (var LiveTrain in Area.LiveTrains.Values)
            {
                if (LiveTrain.Train == null)
                    continue;

                var Train = LiveTrain.Train;

                foreach (var LiveSchedule in LiveTrain.Schedules)
                {
                    if (LiveSchedule.Schedule == null || LiveSchedule.Schedule.Station == null)
                        continue;

                    var Schedule = LiveSchedule.Schedule;
                    var Station = Schedule.Station;

                    bool Visible = true;
                    bool IsNew = false;

                    if (!Runtime.VisibleStations.Contains(Station))
                        Visible = false;

                    LeibitTime ActualTime;

                    if (LiveTrain.Block != null && LiveTrain.Block.Track != null)
                        ActualTime = LiveTrain.Block.Track.Station.ESTW.Time;
                    else
                        ActualTime = Area.ESTWs.Where(e => e.IsLoaded).DefaultIfEmpty().Max(e => e.Time);

                    if (ActualTime != null && LiveTrain.LastModified != null && ActualTime > LiveTrain.LastModified.AddMinutes(1))
                        Visible = false;

                    var Current = Trains.FirstOrDefault(t => t.Station.ShortSymbol == Station.ShortSymbol && t.TrainNumber == Train.Number && t.Arrival == Schedule.Arrival && t.Departure == Schedule.Departure);

                    if (Current == null)
                    {
                        Current = new TrainStationViewModel(LiveTrain, Schedule)
                        {
                            Station = Station,
                            TrainNumber = Train.Number,
                            Arrival = Schedule.Arrival,
                            Departure = Schedule.Departure,
                            Track = Schedule.Track
                        };

                        IsNew = true;
                    }

                    Current.Delay = LiveTrain.Delay;
                    Current.ExpectedArrival = LiveSchedule.ExpectedArrival;
                    Current.ExpectedDeparture = LiveSchedule.ExpectedDeparture;
                    Current.LiveTrack = LiveSchedule.LiveTrack == null || LiveSchedule.LiveTrack.Name == Schedule.Track?.Name ? null : LiveSchedule.LiveTrack;
                    Current.LocalOrders = Schedule.LocalOrders.IsNotNullOrWhiteSpace() ? 'J' : ' ';

                    var NextSchedules = LiveTrain.Schedules.Skip(LiveTrain.Schedules.IndexOf(LiveSchedule) + 1);

                    if (/*(NextSchedules.Count() == 0 && LiveSchedule.LiveDeparture != null) ||*/ NextSchedules.Any(s => s.LiveArrival != null))
                        Visible = false;

                    var Delays = LiveTrain.Schedules.SelectMany(s => s.Delays);
                    var UnjustifiedDelays = Delays.Where(d => d.Reason.IsNullOrWhiteSpace() && d.Schedule.Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)));

                    if (UnjustifiedDelays.Any())
                    {
                        Current.DelayInfo = 'U';

                        if (UnjustifiedDelays.Any(d => d.Schedule.Schedule.Station.ShortSymbol == Schedule.Station.ShortSymbol))
                            Visible = true;
                    }
                    else if (Delays.Any(d => d.Reason.IsNotNullOrWhiteSpace()))
                        Current.DelayInfo = 'J';
                    else
                        Current.DelayInfo = ' ';

                    if (Visible)
                    {
                        if (LiveTrain.Block != null && LiveTrain.Block.Track != null && LiveTrain.Block.Track.Station != null)
                        {
                            Current.CurrentStation = LiveTrain.Block.Track.Station;

                            var CurrentSchedule = LiveTrain.Schedules.LastOrDefault(s => s.LiveArrival != null && s.Schedule != null && s.Schedule.Station != null && s.Schedule.Station.ShortSymbol == LiveTrain.Block.Track.Station.ShortSymbol);

                            if (CurrentSchedule == null)
                                Current.State = LiveSchedule.LiveDeparture == null ? "ab" : "beendet";
                            else
                            {
                                int CurrentScheduleIndex = LiveTrain.Schedules.IndexOf(CurrentSchedule);
                                int LiveScheduleIndex = LiveTrain.Schedules.IndexOf(LiveSchedule);

                                if (CurrentScheduleIndex > LiveScheduleIndex)
                                    Current.State = "beendet";
                                else if (CurrentSchedule.LiveDeparture == null)
                                {
                                    if (CurrentSchedule.Schedule.Handling == eHandling.Start && CurrentSchedule.Schedule.Station.ESTW.Time >= CurrentSchedule.Schedule.Departure.AddMinutes(-2))
                                        Current.State = "fertig";
                                    else
                                        Current.State = "an";
                                }
                                else
                                {
                                    if (CurrentScheduleIndex == LiveScheduleIndex)
                                        Current.State = "beendet";
                                    else
                                        Current.State = "ab";
                                }
                            }
                        }

                        if (IsNew)
                            Dispatcher.Invoke(() => Trains.Add(Current));

                        CurrentTrains.Add(Current);
                    }
                }
            }

            var RemovedTrains = Trains.Except(CurrentTrains).ToList();
            Dispatcher.Invoke(() => RemovedTrains.ForEach(t => Trains.Remove(t)));
        }
        #endregion

        #region [SaveLayout]
        public void SaveLayout()
        {
            SaveGridLayout = true;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__RowDoubleClick]
        private void __RowDoubleClick()
        {
            if (SelectedItem == null)
                return;

            if (SelectedColumn == "DelayInfo" && SelectedItem.DelayInfo == 'U')
            {
                var Window = new DelayJustificationView(SelectedItem.TrainNumber);
                var VM = new DelayJustificationViewModel(SelectedItem.CurrentTrain);
                OnOpenWindow(VM, Window);
            }
            else if (SelectedColumn == "LocalOrders" && SelectedItem.LocalOrders == 'J')
            {
                var Window = new LocalOrdersView(SelectedItem.TrainNumber, SelectedItem.Station.ShortSymbol);
                var VM = new LocalOrdersViewModel(SelectedItem.Schedule);
                OnOpenWindow(VM, Window);
            }
            else
            {
                var Window = new TrainScheduleView(SelectedItem.TrainNumber);
                var VM = new TrainScheduleViewModel(Window.Dispatcher, SelectedItem.CurrentTrain.Train);
                OnOpenWindow(VM, Window);
            }
        }
        #endregion

        #endregion

    }
}
