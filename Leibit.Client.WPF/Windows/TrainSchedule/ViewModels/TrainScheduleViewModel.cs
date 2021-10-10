using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
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
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.TrainSchedule.ViewModels
{

    public class TrainScheduleViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Needs -
        private LiveDataBLL m_LiveDataBll;
        private CalculationBLL m_CalculationBll;
        private SettingsBLL m_SettingsBll;
        private TrainInformation m_LiveTrain;
        private eSkin? m_Skin;
        private Area m_Area;
        #endregion

        #region - Ctor -
        public TrainScheduleViewModel(Dispatcher Dispatcher, Train Train, Area Area)
            : this(Dispatcher, Train, Area, false)
        {

        }

        public TrainScheduleViewModel(Dispatcher Dispatcher, Train Train, Area Area, bool IsInEditMode)
            : base()
        {
            this.IsInEditMode = IsInEditMode;
            this.Dispatcher = Dispatcher;
            CurrentTrain = Train;
            m_Area = Area;
            m_LiveDataBll = new LiveDataBLL();
            m_CalculationBll = new CalculationBLL();
            m_SettingsBll = new SettingsBLL();
            Stations = new ObservableCollection<TrainScheduleStationViewModel>();
            OpenTrainScheduleCommand = new CommandHandler<int?>(__OpenTrainSchedule, true);
            SaveCommand = new CommandHandler(__Save, true);
            CancelTrainCommand = new CommandHandler(__CancelTrain, true);

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
                return $"Zuglauf {CurrentTrain.Type} {CurrentTrain.Number}";
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

        #region [PreviousServiceVisibility]
        public Visibility PreviousServiceVisibility
        {
            get => Get<Visibility>();
            private set => Set(value);
        }
        #endregion

        #region [FollowUpServiceVisibility]
        public Visibility FollowUpServiceVisibility
        {
            get => Get<Visibility>();
            private set => Set(value);
        }
        #endregion

        #region [PreviousTrainNumber]
        public int? PreviousTrainNumber
        {
            get => Get<int?>();
            private set => Set(value);
        }
        #endregion

        #region [FollowUpTrainNumber]
        public int? FollowUpTrainNumber
        {
            get => Get<int?>();
            private set => Set(value);
        }
        #endregion

        #region [OpenTrainScheduleCommand]
        public ICommand OpenTrainScheduleCommand
        {
            get => Get<ICommand>();
            private set => Set(value);
        }
        #endregion

        #region [SaveCommand]
        public ICommand SaveCommand { get; }
        #endregion

        #region [CancelTrainCommand]
        public ICommand CancelTrainCommand { get; }
        #endregion

        #region [IsInEditMode]
        public bool IsInEditMode { get; }
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
            var SchedulesResult = m_CalculationBll.GetSchedulesByTime(CurrentTrain.Schedules, Area.ESTWs.Max(e => e.Time));

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
                    FirstStationSchedule = new TrainScheduleStationViewModel(CurrentTrain.Start, false, IsInEditMode);

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
                    LastStationSchedule = new TrainScheduleStationViewModel(CurrentTrain.Destination, true, IsInEditMode);

                    if (Area.LiveTrains.ContainsKey(CurrentTrain.Number) && Area.LiveTrains[CurrentTrain.Number].IsDestinationStationCancelled)
                        LastStationSchedule.IsCancelled = true;

                    Dispatcher.Invoke(() => Stations.Add(LastStationDummy));
                    Dispatcher.Invoke(() => Stations.Add(LastStationSchedule));
                }

                LastStationDummy.IsCancelled = LastStationSchedule.IsCancelled;
                CurrentSchedules.Add(LastStationDummy);
                CurrentSchedules.Add(LastStationSchedule);
            }

            bool PreviousVisible = false;
            bool DummyFlag = false;
            bool Reorder = false;

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
                        Current = new TrainScheduleStationViewModel(Schedule, IsInEditMode);
                        Current.PropertyChanged += __Station_PropertyChanged;
                        IsNew = true;
                    }
                    else if (Reorder)
                    {
                        Dispatcher.Invoke(() => Stations.Remove(Current));
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

                            if (LiveSchedule.ExpectedArrival != null && Schedule.Arrival != null && !IsInEditMode)
                                Current.DelayArrival = (LiveSchedule.ExpectedArrival - Schedule.Arrival).TotalMinutes;
                            else
                                Current.DelayArrival = null;

                            if (LiveSchedule.ExpectedDeparture != null && Schedule.Departure != null && !IsInEditMode)
                                Current.DelayDeparture = (LiveSchedule.ExpectedDeparture - Schedule.Departure).TotalMinutes;
                            else
                                Current.DelayDeparture = null;

                            Current.IsDelayArrivalManuallySet = LiveSchedule.ExpectedDelayArrival.HasValue && !LiveSchedule.IsArrived;
                            Current.IsDelayDepartureManuallySet = LiveSchedule.ExpectedDelayDeparture.HasValue && !LiveSchedule.IsDeparted;

                            if (IsNew)
                                Current.IsCancelled = LiveSchedule.IsCancelled;

                            var Index = m_LiveTrain.Schedules.IndexOf(LiveSchedule);

                            if (m_LiveTrain.Schedules.Where((schedule, index) => index > Index && (schedule.Schedule.Station.ShortSymbol != Schedule.Station.ShortSymbol || schedule.Schedule.Time != Schedule.Time) && schedule.IsArrived).Any())
                            {
                                Current.IsArrived = true;
                                Current.IsDeparted = true;

                                var isSkipped = !LiveSchedule.IsArrived && !LiveSchedule.IsDeparted;

                                if (Current.IsSkipped && !isSkipped)
                                {
                                    // Trigger re-ordering of this and all subsequent entries
                                    Dispatcher.Invoke(() => Stations.Remove(Current));
                                    IsNew = true;
                                    Reorder = true;
                                }

                                Current.IsSkipped = isSkipped;
                            }
                            else
                            {
                                Current.IsArrived = LiveSchedule.IsArrived;
                                Current.IsDeparted = LiveSchedule.IsDeparted;
                            }

                            Current.LiveTrack = LiveSchedule.LiveTrack;

                            var Minutes = LiveSchedule.Delays.Sum(d => d.Minutes);

                            if (LiveSchedule.Delays.Any(d => d.Reason.IsNullOrWhiteSpace()) && Schedule.Station.ESTW.Stations.Any(s => Runtime.VisibleStations.Contains(s)) && !IsInEditMode)
                            {
                                Current.DelayInfo = String.Format("+{0} Bitte begründen!", Minutes);
                                Dispatcher.Invoke(() => Current.IsDelayJustified = false);
                            }
                            else if (LiveSchedule.Delays.Any(d => d.Reason.IsNotNullOrWhiteSpace()) && !IsInEditMode)
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
                    Current.CanCancel = IsInEditMode && !Current.IsArrived && !Current.IsDeparted;

                    if (Visible)
                    {
                        if (IsNew)
                        {
                            int Index = -1;

                            if (Current.LiveTime != null)
                            {
                                for (int i = 0; i < Stations.Count; i++)
                                {
                                    if (Stations[i].HasSchedule && Stations[i].LiveTime > Current.LiveTime)
                                    {
                                        Index = i;
                                        break;
                                    }
                                }
                            }

                            if (Index == -1)
                            {
                                for (int i = 0; i < Stations.Count; i++)
                                {
                                    if (Stations[i].HasSchedule && Stations[i].CurrentSchedule.Time > Current.CurrentSchedule.Time)
                                    {
                                        Index = i;
                                        break;
                                    }
                                }
                            }

                            if (Index == -1)
                                Index = Stations.Count;

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

            if (HasStartStation && !IsInEditMode)
            {
                if (m_LiveTrain == null)
                {
                    var startSchedule = SchedulesResult.Result.First(s => s.Handling == eHandling.Start);
                    var prevResult = m_CalculationBll.GetPreviousService(CurrentTrain, startSchedule.Station.ESTW);

                    if (prevResult.Succeeded && prevResult.Result.HasValue)
                    {
                        PreviousTrainNumber = prevResult.Result;
                        PreviousServiceVisibility = Visibility.Visible;
                    }
                    else
                        PreviousServiceVisibility = Visibility.Collapsed;
                }
                else if (m_LiveTrain.PreviousService.HasValue)
                {
                    PreviousTrainNumber = m_LiveTrain.PreviousService;
                    PreviousServiceVisibility = Visibility.Visible;
                }
                else
                    PreviousServiceVisibility = Visibility.Collapsed;
            }
            else
                PreviousServiceVisibility = Visibility.Collapsed;

            if (HasDestinationStation && !IsInEditMode)
            {
                if (m_LiveTrain == null)
                {
                    var startSchedule = SchedulesResult.Result.First(s => s.Handling == eHandling.Destination);
                    var followUpResult = m_CalculationBll.GetFollowUpService(CurrentTrain, startSchedule.Station.ESTW);

                    if (followUpResult.Succeeded && followUpResult.Result.HasValue)
                    {
                        FollowUpTrainNumber = followUpResult.Result;
                        FollowUpServiceVisibility = Visibility.Visible;
                    }
                    else
                        FollowUpServiceVisibility = Visibility.Collapsed;
                }
                else if (m_LiveTrain.FollowUpService.HasValue)
                {
                    FollowUpTrainNumber = m_LiveTrain.FollowUpService;
                    FollowUpServiceVisibility = Visibility.Visible;
                }
                else
                    FollowUpServiceVisibility = Visibility.Collapsed;
            }
            else
                FollowUpServiceVisibility = Visibility.Collapsed;

            if (Settings.Skin != m_Skin)
            {
                m_Skin = Settings.Skin;
                Stations.ForEach(s => s.SkinChanged());
            }
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

        #region [__OpenTrainSchedule]
        private void __OpenTrainSchedule(int? trainNumber)
        {
            if (!trainNumber.HasValue || m_Area == null || !m_Area.Trains.ContainsKey(trainNumber.Value))
                return;

            var Window = new TrainScheduleView(trainNumber.Value);
            var VM = new TrainScheduleViewModel(Window.Dispatcher, m_Area.Trains[trainNumber.Value], m_Area);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #region [__Save]
        private void __Save()
        {
            var estw = m_Area.ESTWs.FirstOrDefault(e => e.Time == m_Area.ESTWs.Max(e => e.Time));
            TrainInformation liveTrain;

            if (m_Area.LiveTrains.ContainsKey(CurrentTrain.Number))
                liveTrain = m_Area.LiveTrains[CurrentTrain.Number];
            else
            {
                var createResult = m_LiveDataBll.CreateLiveTrainInformation(CurrentTrain.Number, estw);

                if (createResult.Succeeded)
                    liveTrain = m_Area.LiveTrains.GetOrAdd(CurrentTrain.Number, createResult.Result);
                else
                {
                    MessageBox.Show(createResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            liveTrain.LastModified = estw.Time;

            foreach (var station in Stations)
            {
                liveTrain.Schedules.Where(s => s.Schedule.Station.ShortSymbol == station.CurrentSchedule?.Station.ShortSymbol && s.Schedule.Time == station.CurrentSchedule?.Time)
                                   .ForEach(s => s.IsCancelled = station.IsCancelled);

                if (station.IsLastStation)
                    liveTrain.IsDestinationStationCancelled = station.IsCancelled;
            }

            OnCloseWindow();
        }
        #endregion

        #region [__CancelTrain]
        private void __CancelTrain()
        {
            Stations.Where(s => s.CanCancel).ForEach(s => s.IsCancelled = true);
        }
        #endregion

        #endregion

    }

}
