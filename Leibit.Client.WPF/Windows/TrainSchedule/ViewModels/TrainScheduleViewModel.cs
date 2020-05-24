using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.TrainSchedule.ViewModels
{

    public class TrainScheduleViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Needs -
        private CalculationBLL m_CalculationBll;
        private SettingsBLL m_SettingsBll;
        private TrainInformation m_LiveTrain;
        #endregion

        #region - Ctor -
        public TrainScheduleViewModel(Dispatcher Dispatcher, Train Train, Area Area)
            : base()
        {
            this.Dispatcher = Dispatcher;
            CurrentTrain = Train;
            m_CalculationBll = new CalculationBLL();
            m_SettingsBll = new SettingsBLL();
            Stations = new ObservableCollection<TrainScheduleStationViewModel>();

            if (Area != null)
                Refresh(Area);
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption
        {
            get
            {
                return String.Format("Zuglauf {0} {1}", CurrentTrain.Type, CurrentTrain.Number);
            }
        }
        #endregion

        #region [CurrentTrain]
        public Train CurrentTrain
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

        #region [Stations]
        public ObservableCollection<TrainScheduleStationViewModel> Stations
        {
            get
            {
                return Get<ObservableCollection<TrainScheduleStationViewModel>>();
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
            var SettingsResult = m_SettingsBll.GetSettings();

            if (!SettingsResult.Succeeded)
            {
                MessageBox.Show(SettingsResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var Settings = SettingsResult.Result;
            var CurrentSchedules = new List<TrainScheduleStationViewModel>();
            var Estw = Area.ESTWs.FirstOrDefault(estw => estw.Time != null);

            if (Estw == null)
                return;

            var SchedulesResult = m_CalculationBll.GetSchedulesByTime(CurrentTrain.Schedules, Estw.Time);

            if (!SchedulesResult.Succeeded)
            {
                Dispatcher.Invoke(() => MessageBox.Show(SchedulesResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
                return;
            }

            bool HasStartStation = SchedulesResult.Result.Any(s => s.Handling == eHandling.Start);
            bool HasDestinationStation = SchedulesResult.Result.Any(s => s.Handling == eHandling.Destination);

            TrainScheduleStationViewModel FirstStationSchedule = null;
            TrainScheduleStationViewModel FirstStationDummy = null;
            TrainScheduleStationViewModel LastStationSchedule = null;
            TrainScheduleStationViewModel LastStationDummy = null;

            if (!HasStartStation && CurrentTrain.Start != null)
            {
                FirstStationSchedule = Stations.FirstOrDefault();
                FirstStationDummy = Stations.ElementAtOrDefault(1);

                if (FirstStationSchedule == null || FirstStationSchedule.HasSchedule)
                {
                    FirstStationDummy = new TrainScheduleStationViewModel();
                    FirstStationSchedule = new TrainScheduleStationViewModel(CurrentTrain.Start, false);

                    Dispatcher.Invoke(() => Stations.Insert(0, FirstStationDummy));
                    Dispatcher.Invoke(() => Stations.Insert(0, FirstStationSchedule));
                }

                CurrentSchedules.Add(FirstStationDummy);
                CurrentSchedules.Add(FirstStationSchedule);
            }

            if (!HasDestinationStation && CurrentTrain.Destination != null)
            {
                LastStationSchedule = Stations.LastOrDefault();
                LastStationDummy = Stations.ElementAtOrDefault(Stations.Count - 2);

                if (LastStationSchedule == null || LastStationSchedule.HasSchedule || LastStationSchedule == FirstStationDummy)
                {
                    LastStationDummy = new TrainScheduleStationViewModel();
                    LastStationSchedule = new TrainScheduleStationViewModel(CurrentTrain.Destination, true);

                    Dispatcher.Invoke(() => Stations.Add(LastStationDummy));
                    Dispatcher.Invoke(() => Stations.Add(LastStationSchedule));
                }

                CurrentSchedules.Add(LastStationDummy);
                CurrentSchedules.Add(LastStationSchedule);
            }

            bool PreviousVisible = false;
            bool DummyFlag = false;

            // Group schedules by station and time to avoid duplicate entries for stations that are located between ESTW bordery stations when both ESTWs are loaded
            // i.e. Bosserode and Obersuhl are located between Hönebach (ESTW Bebra) and Gerstungen (ESTW Eisenach)
            foreach (var ScheduleGroup in SchedulesResult.Result.GroupBy(s => new { s.Station.ShortSymbol, s.Time }))
            {
                bool GroupVisible = false;

                foreach (var Schedule in ScheduleGroup)
                {
                    if (Schedule.Station == null)
                        continue;

                    bool Visible = true;
                    bool IsNew = false;

                    var Current = Stations.FirstOrDefault(s => s.CurrentSchedule == Schedule);

                    if (Current == null)
                    {
                        Current = new TrainScheduleStationViewModel(Schedule);
                        Current.PropertyChanged += __Station_PropertyChanged;
                        IsNew = true;
                    }

                    Current.Departure = Schedule.Departure; // For special trains schedule is generated at runtime

                    if (!Settings.DisplayCompleteTrainSchedule && !Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)))
                        Visible = Schedule.Handling == eHandling.Start || Schedule.Handling == eHandling.Destination;

                    if (Area.LiveTrains.ContainsKey(CurrentTrain.Number))
                    {
                        m_LiveTrain = Area.LiveTrains[CurrentTrain.Number];
                        var LiveSchedule = m_LiveTrain.Schedules.FirstOrDefault(s => s.Schedule == Schedule);

                        if (LiveSchedule != null)
                        {
                            if (FirstStationSchedule != null)
                            {
                                FirstStationSchedule.IsArrived = m_LiveTrain.Schedules.Any(s => s.IsArrived);
                                FirstStationSchedule.IsDeparted = FirstStationSchedule.IsArrived;
                            }

                            if (FirstStationDummy != null)
                            {
                                FirstStationDummy.IsArrived = m_LiveTrain.Schedules.Any(s => s.IsArrived);
                                FirstStationDummy.IsDeparted = FirstStationDummy.IsArrived;
                            }

                            if (LiveSchedule.ExpectedArrival != null && Schedule.Arrival != null)
                                Current.DelayArrival = (LiveSchedule.ExpectedArrival - Schedule.Arrival).TotalMinutes;

                            if (LiveSchedule.ExpectedDeparture != null && Schedule.Departure != null)
                                Current.DelayDeparture = (LiveSchedule.ExpectedDeparture - Schedule.Departure).TotalMinutes;

                            var Index = m_LiveTrain.Schedules.IndexOf(LiveSchedule);

                            if (m_LiveTrain.Schedules.Where((schedule, index) => index > Index && (schedule.Schedule.Station.ShortSymbol != Schedule.Station.ShortSymbol || schedule.Schedule.Time != Schedule.Time) && schedule.IsArrived).Any())
                            {
                                Current.IsArrived = true;
                                Current.IsDeparted = true;
                            }
                            else
                            {
                                Current.IsArrived = LiveSchedule.IsArrived;
                                Current.IsDeparted = LiveSchedule.IsDeparted;
                            }

                            Current.LiveTrack = LiveSchedule.LiveTrack;

                            var Minutes = LiveSchedule.Delays.Sum(d => d.Minutes);

                            if (LiveSchedule.Delays.Any(d => d.Reason.IsNullOrWhiteSpace()) && Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)))
                            {
                                Current.DelayInfo = String.Format("+{0} Bitte begründen!", Minutes);
                                Dispatcher.Invoke(() => Current.IsDelayJustified = false);
                            }
                            else if (LiveSchedule.Delays.Any(d => d.Reason.IsNotNullOrWhiteSpace()))
                            {
                                Current.DelayInfo = String.Format("+{0} {1}", Minutes, String.Join(", ", LiveSchedule.Delays.Where(d => d.Reason.IsNotNullOrWhiteSpace()).Select(d =>
                                {
                                    string result = d.Reason;

                                    if (d.CausedBy.HasValue)
                                        result += String.Format(" ({0})", d.CausedBy);

                                    return result;
                                })));

                                Dispatcher.Invoke(() => Current.IsDelayJustified = true);
                            }
                            else
                                Current.DelayInfo = null;
                        }
                    }

                    GroupVisible |= Visible;

                    if (Visible)
                    {
                        if (IsNew)
                        {
                            int Index = -1;
                            int i;

                            for (i = 0; i < Stations.Count; i++)
                            {
                                if (Stations[i].HasSchedule && Stations[i].CurrentSchedule.Time > Current.CurrentSchedule.Time)
                                {
                                    Index = i;
                                    break;
                                }
                            }

                            if (Index == -1)
                                Index = i;

                            Dispatcher.Invoke(() => Stations.Insert(Index, Current));
                        }

                        CurrentSchedules.Add(Current);

                        if (DummyFlag)
                        {
                            var Index = Stations.IndexOf(Current);
                            var Dummy = Stations.ElementAtOrDefault(Index - 1);

                            if (Dummy == null || Dummy.HasStation)
                            {
                                Dummy = new TrainScheduleStationViewModel();
                                Dispatcher.Invoke(() => Stations.Insert(Index, Dummy));
                            }

                            Dummy.IsArrived = Current.IsArrived;
                            Dummy.IsDeparted = Current.IsArrived;

                            CurrentSchedules.Add(Dummy);
                            DummyFlag = false;
                        }

                        break;
                    }
                }

                if (!GroupVisible && PreviousVisible)
                    DummyFlag = true;

                PreviousVisible = GroupVisible;
            }

            var RemovedItems = Stations.Except(CurrentSchedules).ToList();
            Dispatcher.Invoke(() => RemovedItems.ForEach(s => Stations.Remove(s)));
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Station_PropertyChanged]
        private void __Station_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var SenderVM = sender as TrainScheduleStationViewModel;

            if (SenderVM == null)
                return;

            if (e.PropertyName == "JustifyDelay" && m_LiveTrain != null)
            {
                var Window = new DelayJustificationView(CurrentTrain.Number);
                var VM = new DelayJustificationViewModel(m_LiveTrain);

                OnOpenWindow(VM, Window);
            }

            if (e.PropertyName == "ShowLocalOrders")
            {
                var Window = new LocalOrdersView(CurrentTrain.Number, SenderVM.CurrentSchedule.Station.ShortSymbol);
                var VM = new LocalOrdersViewModel(SenderVM.CurrentSchedule);

                OnOpenWindow(VM, Window);
            }
        }
        #endregion

        #endregion

    }

}
