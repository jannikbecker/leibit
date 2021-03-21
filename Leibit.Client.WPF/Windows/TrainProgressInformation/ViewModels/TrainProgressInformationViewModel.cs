using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Dialogs.TrainNumber;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
using Leibit.Client.WPF.Windows.ExpectedDelay.ViewModels;
using Leibit.Client.WPF.Windows.ExpectedDelay.Views;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Client.WPF.Windows.TrackChange.ViewModels;
using Leibit.Client.WPF.Windows.TrackChange.Views;
using Leibit.Client.WPF.Windows.TrainComposition.ViewModels;
using Leibit.Client.WPF.Windows.TrainComposition.Views;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Client.WPF.Windows.TrainState.ViewModels;
using Leibit.Client.WPF.Windows.TrainState.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.TrainProgressInformation.ViewModels
{
    public class TrainProgressInformationViewModel : ChildWindowViewModelBase, IRefreshable, ILayoutSavable
    {

        #region - Needs -
        private CommandHandler m_DoubleClickCommand;
        private SettingsBLL m_SettingsBll;
        private CalculationBLL m_CalculationBll;
        private Area m_Area;
        #endregion

        #region - Ctor -
        public TrainProgressInformationViewModel(Dispatcher Dispatcher, Area area)
        {
            this.Dispatcher = Dispatcher;
            Trains = new ObservableCollection<TrainStationViewModel>();
            m_SettingsBll = new SettingsBLL();
            m_CalculationBll = new CalculationBLL();
            m_Area = area;

            m_DoubleClickCommand = new CommandHandler(__RowDoubleClick, true);
            EnterExpectedDelayCommand = new CommandHandler(__EnterExpectedDelay, false);
            ShowTrainScheduleCommand = new CommandHandler(__ShowTrainSchedule, false);
            ShowTrackChangeCommand = new CommandHandler(__ShowTrackChange, false);
            ShowLocalOrdersCommand = new CommandHandler(__ShowLocalOrders, false);
            ShowTrainCompositionCommand = new CommandHandler(__ShowTrainComposition, false);
            ShowDelayJustificationCommand = new CommandHandler(__ShowDelayJustification, false);
            NewTrainStateCommand = new CommandHandler(__NewTrainState, true);
            EnterTrainStateCommand = new CommandHandler(__EnterTrainState, false);
            NewCommand = new CommandHandler(__NewEntry, true);
            DeleteCommand = new CommandHandler(__DeleteEntry, false);
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

        #region [NeedsRefresh]
        public bool NeedsRefresh
        {
            get;
            set;
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

        #region [EnterExpectedDelayCommand]
        public CommandHandler EnterExpectedDelayCommand { get; }
        #endregion

        #region [ShowTrackChangeCommand]
        public CommandHandler ShowTrackChangeCommand { get; }
        #endregion

        #region [NewTrainStateCommand]
        public CommandHandler NewTrainStateCommand { get; }
        #endregion

        #region [EnterTrainStateCommand]
        public CommandHandler EnterTrainStateCommand { get; }
        #endregion

        #region [ShowTrainScheduleCommand]
        public CommandHandler ShowTrainScheduleCommand { get; }
        #endregion

        #region [ShowLocalOrdersCommand]
        public CommandHandler ShowLocalOrdersCommand { get; }
        #endregion

        #region [ShowTrainCompositionCommand]
        public CommandHandler ShowTrainCompositionCommand { get; }
        #endregion

        #region [ShowDelayJustificationCommand]
        public CommandHandler ShowDelayJustificationCommand { get; }
        #endregion

        #region [NewCommand]
        public CommandHandler NewCommand { get; }
        #endregion

        #region [DeleteCommand]
        public CommandHandler DeleteCommand { get; }
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
                __RefreshCommands();
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
            __CleanupState(Area);
            var settingsResult = m_SettingsBll.GetSettings();

            if (!settingsResult.Succeeded)
            {
                MessageBox.Show(settingsResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var settings = settingsResult.Result;
            var currentTrains = new List<TrainStationViewModel>();
            var stations = Runtime.VisibleStations.Select(s => s.ESTW).Distinct().SelectMany(e => e.Stations).ToList();

            foreach (var station in stations)
            {
                var schedulesResult = m_CalculationBll.GetSchedulesByTime(station.Schedules, station.ESTW.Time);

                if (!schedulesResult.Succeeded)
                {
                    Dispatcher.Invoke(() => MessageBox.Show(schedulesResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
                    continue;
                }

                foreach (var schedule in schedulesResult.Result)
                {
                    var train = schedule.Train;
                    Area.LiveTrains.TryGetValue(train.Number, out var liveTrain);
                    var liveSchedule = liveTrain?.Schedules.FirstOrDefault(s => s.Schedule == schedule);

                    if (__IsVisible(schedule, liveSchedule, settings))
                    {
                        var vm = __BuildEntry(schedule, liveSchedule, settings);
                        currentTrains.Add(vm);
                    }
                }
            }

            var removedTrains = Trains.Except(currentTrains).ToList();
            Dispatcher.Invoke(() => removedTrains.ForEach(t => Trains.Remove(t)));
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

        #region [__RefreshCommands]
        private void __RefreshCommands()
        {
            if (SelectedItem == null)
            {
                EnterExpectedDelayCommand.SetCanExecute(false);
                ShowTrackChangeCommand.SetCanExecute(false);
                ShowTrainScheduleCommand.SetCanExecute(false);
                ShowLocalOrdersCommand.SetCanExecute(false);
                ShowTrainCompositionCommand.SetCanExecute(false);
                ShowDelayJustificationCommand.SetCanExecute(false);
                EnterTrainStateCommand.SetCanExecute(false);
                DeleteCommand.SetCanExecute(false);
            }
            else
            {
                ShowTrainScheduleCommand.SetCanExecute(true);
                DeleteCommand.SetCanExecute(true);
                ShowTrainCompositionCommand.SetCanExecute(SelectedItem.Schedule.Train.Composition.IsNotNullOrWhiteSpace());
                ShowLocalOrdersCommand.SetCanExecute(SelectedItem.Schedule.LocalOrders.IsNotNullOrWhiteSpace());

                if (SelectedItem.CurrentTrain == null)
                {
                    EnterExpectedDelayCommand.SetCanExecute(false);
                    ShowTrackChangeCommand.SetCanExecute(false);
                    ShowDelayJustificationCommand.SetCanExecute(false);
                    EnterTrainStateCommand.SetCanExecute(false);
                }
                else
                {
                    var liveSchedule = SelectedItem.CurrentTrain.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == SelectedItem.Schedule.Station.ShortSymbol && s.Schedule.Time == SelectedItem.Schedule.Time);

                    EnterExpectedDelayCommand.SetCanExecute(SelectedItem.Schedule.Handling != eHandling.Destination && SelectedItem.State != "beendet");
                    ShowTrackChangeCommand.SetCanExecute(liveSchedule != null && !liveSchedule.IsArrived && (SelectedItem.Schedule.Track == null || SelectedItem.Schedule.Track.IsPlatform));
                    ShowDelayJustificationCommand.SetCanExecute(SelectedItem.DelayInfo == 'U');
                    EnterTrainStateCommand.SetCanExecute(liveSchedule.IsArrived && !liveSchedule.IsDeparted);
                }
            }
        }
        #endregion

        #region [__RowDoubleClick]
        private void __RowDoubleClick()
        {
            if (SelectedItem == null)
                return;

            if (SelectedColumn == "DelayInfo" && SelectedItem.DelayInfo == 'U')
                __ShowDelayJustification();
            else if (SelectedColumn == "LocalOrders" && SelectedItem.LocalOrders == 'J')
                __ShowLocalOrders();
            else
                __ShowTrainSchedule();
        }
        #endregion

        #region [__EnterExpectedDelay]
        private void __EnterExpectedDelay()
        {
            var window = new ExpectedDelayView(SelectedItem.TrainNumber);
            var vm = new ExpectedDelayViewModel(SelectedItem.CurrentTrain, SelectedItem.Schedule);
            OnOpenWindow(vm, window);
        }
        #endregion

        #region [__ShowTrackChange]
        private void __ShowTrackChange()
        {
            var Window = new TrackChangeView(SelectedItem.TrainNumber);
            var VM = new TrackChangeViewModel(SelectedItem.CurrentTrain, SelectedItem.Schedule);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__NewTrainState]
        private void __NewTrainState()
        {
            if (SelectedItem == null)
                return;

            var window = new TrainStateView();
            var vm = new TrainStateViewModel(SelectedItem.Station.ESTW.Area, null);
            OnOpenWindow(vm, window);
        }
        #endregion

        #region [__EnterTrainState]
        private void __EnterTrainState()
        {
            var window = new TrainStateView();
            var vm = new TrainStateViewModel(SelectedItem.Station.ESTW.Area, SelectedItem.TrainNumber);
            OnOpenWindow(vm, window);
        }
        #endregion

        #region [__ShowTrainSchedule]
        private void __ShowTrainSchedule()
        {
            var Window = new TrainScheduleView(SelectedItem.TrainNumber);
            var VM = new TrainScheduleViewModel(Window.Dispatcher, SelectedItem.Schedule.Train, SelectedItem.Station.ESTW.Area);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__ShowLocalOrders]
        private void __ShowLocalOrders()
        {
            var Window = new LocalOrdersView(SelectedItem.TrainNumber, SelectedItem.Station.ShortSymbol);
            var VM = new LocalOrdersViewModel(SelectedItem.Schedule);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__ShowTrainComposition]
        private void __ShowTrainComposition()
        {
            var Window = new TrainCompositionView(SelectedItem.TrainNumber);
            var VM = new TrainCompositionViewModel(SelectedItem.Schedule.Train);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__ShowDelayJustification]
        private void __ShowDelayJustification()
        {
            var Window = new DelayJustificationView(SelectedItem.TrainNumber);
            var VM = new DelayJustificationViewModel(SelectedItem.CurrentTrain);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__NewEntry]
        private void __NewEntry()
        {
            var trainNumber = TrainNumberDialog.ShowModal();

            if (!trainNumber.HasValue)
                return;

            if (!m_Area.Trains.ContainsKey(trainNumber.Value))
                MessageBox.Show($"Unbekannte Zugnummer: {trainNumber}", "ZFI", MessageBoxButton.OK, MessageBoxImage.Warning);

            Runtime.HiddenSchedules.RemoveAll(x => x.Schedule.Train.Number == trainNumber.Value);

            if (Runtime.VisibleTrains.Exists(x => x.TrainNumber == trainNumber.Value))
                return;

            var entry = new VisibleTrainInfo();
            entry.TrainNumber = trainNumber.Value;
            entry.HadLiveData = m_Area.LiveTrains.ContainsKey(trainNumber.Value);
            Runtime.VisibleTrains.Add(entry);
        }
        #endregion

        #region [__DeleteEntry]
        private void __DeleteEntry()
        {
            if (SelectedItem == null)
                return;

            var entry = new HiddenScheduleInfo();
            entry.Schedule = SelectedItem.Schedule;

            var estwTime = SelectedItem.Station.ESTW.Time;
            entry.CreatedOn = new LeibitTime(estwTime.Hour, estwTime.Minute); // Remove day component from ESTW time.

            if (Runtime.HiddenSchedules.Exists(x => __AreSchedulesEqual(x.Schedule, entry.Schedule)))
                return;

            Runtime.HiddenSchedules.Add(entry);
        }
        #endregion

        #region [__AreSchedulesEqual]
        private bool __AreSchedulesEqual(Schedule schedule1, Schedule schedule2)
        {
            return schedule1.Train.Number == schedule2.Train.Number
                && schedule1.Station.ShortSymbol == schedule2.Station.ShortSymbol
                && schedule1.Time == schedule2.Time;
        }
        #endregion

        #region [__IsVisible]
        private bool __IsVisible(Schedule schedule, LiveSchedule liveSchedule, Entities.Settings.Settings settings)
        {
            // Show always if there is an unjustified delay
            if (liveSchedule != null && liveSchedule.Delays.Any(d => d.Reason.IsNullOrWhiteSpace()) && liveSchedule.Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)))
                return true;

            // If the current station is not selected, don't show
            if (!Runtime.VisibleStations.Contains(schedule.Station))
                return false;

            if (Runtime.HiddenSchedules.Exists(x => __AreSchedulesEqual(x.Schedule, schedule)))
                return false;

            LeibitTime referenceTime;

            if (liveSchedule == null)
            {
                if (Runtime.VisibleTrains.Any(x => x.TrainNumber == schedule.Train.Number))
                    return true;

                // When no lead time is set, display only trains with live data.
                if (!settings.LeadTime.HasValue)
                    return false;

                referenceTime = schedule.Arrival ?? schedule.Departure;
                var diff = (referenceTime - schedule.Station.ESTW.Time).TotalMinutes;

                // Don't display data more than 6 hours in the past.
                if (diff < -360)
                    return false;

                /* Trains without live data.

                Examples:
                - Simulation started at 4:00
                - Current ESTW time is 6:00
                - Lead time is 60 minutes

                Scheduled time | Visible | Reason
                ===============|=========|=============================================================================================
                3:00           | NO      | Scheduled time is before the start of the simulation. This train will likely never appear.
                5:00           | YES     | Train has no live data, but should be visible, e.g. the train was cancelled. This entry can be manually removed.
                6:30           | YES     | Inside lead time
                8:00           | NO      | Outside lead time (should already be ensured by above code)
                */

                // Check if inside lead time (example 3).
                if (diff >= 0 && diff <= settings.LeadTime)
                    return true;

                // Check if the time is between the simulation start time and the current ESTW time (example 2).
                if (referenceTime > schedule.Station.ESTW.StartTime && referenceTime <= schedule.Station.ESTW.Time)
                    return true;

                // Example 1 and 4.
                return false;
            }

            var liveScheduleIndex = liveSchedule.Train.Schedules.IndexOf(liveSchedule);
            var nextSchedules = liveSchedule.Train.Schedules.Skip(liveScheduleIndex + 1);

            // No live data is available for the current station, but the train has already arrived at one of the next stations.
            // So the current station has been skipped.
            // This is the case at the beginning of the simulation or for diverted trains.
            // To ensure that this record will not remain in the view indefinitly, it is made invisible.
            if (!liveSchedule.IsDeparted && nextSchedules.Any(s => !__AreSchedulesEqual(s.Schedule, liveSchedule.Schedule) && s.IsArrived))
                return false;

            // Check follow-up time
            referenceTime = null;

            // Don't check for IsArrived here.
            // In destination stations train numbers are often switched automatically so that from Leibit's view the train is never arrived at its destination station.
            if (liveSchedule.Schedule.Handling == eHandling.Destination)
                referenceTime = liveSchedule.ExpectedArrival;
            else if (liveSchedule.IsDeparted)
                referenceTime = liveSchedule.ExpectedDeparture;

            if (referenceTime != null)
            {
                // FollowUpTime = null should be treated as 0.
                // In this case, this entry should disappear immediately after state "beendet" is reached.
                if (!settings.FollowUpTime.HasValue)
                    return false;

                var diff = (schedule.Station.ESTW.Time - referenceTime).TotalMinutes;

                // Don't display data more than 6 hours in the future.
                if (diff < -360)
                    return false;

                if (diff > settings.FollowUpTime)
                    return false;
            }

            if (Runtime.VisibleTrains.Any(x => x.TrainNumber == schedule.Train.Number))
                return true;

            // Check lead time
            if (settings.LeadTime.HasValue)
            {
                referenceTime = schedule.Arrival ?? schedule.Departure;

                if (liveSchedule.Schedule.Handling == eHandling.Start)
                {
                    if (liveSchedule.ExpectedDeparture != null && liveSchedule.ExpectedDeparture < referenceTime)
                        referenceTime = liveSchedule.ExpectedDeparture;
                }
                else
                {
                    if (liveSchedule.ExpectedArrival != null && liveSchedule.ExpectedArrival < referenceTime)
                        referenceTime = liveSchedule.ExpectedArrival;
                }

                if ((referenceTime - schedule.Station.ESTW.Time).TotalMinutes > settings.LeadTime)
                    return false;
            }

            return true;
        }
        #endregion

        #region [__BuildEntry]
        private TrainStationViewModel __BuildEntry(Schedule schedule, LiveSchedule liveSchedule, Entities.Settings.Settings settings)
        {
            var currentVm = Trains.FirstOrDefault(t => t.Station.ShortSymbol == schedule.Station.ShortSymbol && t.TrainNumber == schedule.Train.Number && t.Schedule.Time == schedule.Time);
            var isNew = false;

            if (currentVm == null)
            {
                currentVm = new TrainStationViewModel(schedule);
                isNew = true;
            }

            if (isNew)
                Dispatcher.Invoke(() => Trains.Add(currentVm));


            if (liveSchedule != null)
            {
                currentVm.CurrentTrain = liveSchedule.Train;
                currentVm.ExpectedArrival = liveSchedule.ExpectedArrival;
                currentVm.ExpectedDeparture = liveSchedule.ExpectedDeparture;

                if (liveSchedule.LiveTrack != null && liveSchedule.LiveTrack.Name != schedule.Track?.Name)
                    currentVm.LiveTrack = liveSchedule.LiveTrack;
                else
                    currentVm.LiveTrack = null;

                var delays = liveSchedule.Train.Schedules.SelectMany(s => s.Delays);
                var unjustifiedDelays = delays.Where(d => d.Reason.IsNullOrWhiteSpace() && d.Schedule.Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)));

                if (unjustifiedDelays.Any())
                    currentVm.DelayInfo = 'U';
                else if (delays.Any(d => d.Reason.IsNotNullOrWhiteSpace()))
                    currentVm.DelayInfo = 'J';
                else
                    currentVm.DelayInfo = ' ';

                if (liveSchedule.Train.Block == null)
                {
                    currentVm.CurrentStation = null;
                    currentVm.State = string.Empty;
                    currentVm.Delay = null;
                }
                else
                {
                    currentVm.Delay = liveSchedule.Train.Delay;
                    currentVm.CurrentStation = liveSchedule.Train.Block.Track.Station;

                    // Determine state
                    var currentSchedule = liveSchedule.Train.Schedules.LastOrDefault(s => s.IsArrived && s.Schedule.Station.ShortSymbol == liveSchedule.Train.Block.Track.Station.ShortSymbol);

                    var isFirstStation = liveSchedule.Train.BlockHistory.Select(b => b.Track.Station).Distinct().Count() == 1
                        && liveSchedule.Train.Block.Track.CalculateDelay
                        && liveSchedule.Train.Block.Track.IsPlatform;

                    if (currentSchedule?.Schedule.Departure == null)
                        isFirstStation = false;
                    else if (currentSchedule?.Schedule.Handling == eHandling.Start)
                        isFirstStation = true;
                    else if (liveSchedule.Train.CreatedOn == liveSchedule.Train.Block.Track.Station.ESTW.StartTime)
                        isFirstStation = false;

                    if (liveSchedule.IsDeparted)
                        currentVm.State = "beendet";
                    else if (liveSchedule.Schedule.Handling == eHandling.Destination && liveSchedule.IsArrived)
                        currentVm.State = "beendet";
                    else if (currentSchedule == null)
                        currentVm.State = "ab";
                    else if (currentSchedule.IsDeparted)
                        currentVm.State = "ab";
                    else if (isFirstStation && settings.AutomaticReadyMessageEnabled && currentSchedule.Schedule.Station.ESTW.Time >= currentSchedule.Schedule.Departure.AddMinutes(-settings.AutomaticReadyMessageTime))
                        currentVm.State = "fertig";
                    else if (currentSchedule.IsPrepared)
                        currentVm.State = "vorbereitet";
                    else if (currentSchedule.IsComposed)
                        currentVm.State = "bereitgestellt";
                    else if (isFirstStation)
                    {
                        currentVm.CurrentStation = null;
                        currentVm.State = string.Empty;
                        currentVm.Delay = null;
                    }
                    else
                        currentVm.State = "an";
                }
            }

            return currentVm;
        }
        #endregion

        #region [__CleanupState]
        private void __CleanupState(Area area)
        {
            foreach (var train in Runtime.VisibleTrains)
                if (area.LiveTrains.ContainsKey(train.TrainNumber))
                    train.HadLiveData = true;

            Runtime.VisibleTrains.RemoveAll(x => x.HadLiveData && !area.LiveTrains.ContainsKey(x.TrainNumber));

            Runtime.HiddenSchedules.RemoveAll(hiddenSchedule =>
            {
                var estw = hiddenSchedule.Schedule.Station.ESTW;

                if (estw.Time < hiddenSchedule.Schedule.Time)
                {
                    if (hiddenSchedule.CreatedOn < hiddenSchedule.Schedule.Time)
                        return estw.Time < hiddenSchedule.CreatedOn;
                    else
                        return true;
                }

                return false;
            });
        }
        #endregion

        #endregion

    }
}
