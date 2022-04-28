using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.TimeTable.ViewModels
{
    public class TimeTableViewModel : ChildWindowViewModelBase, IRefreshable, ILayoutSavable
    {

        #region - Needs -
        private CalculationBLL m_CalculationBll;
        private CommandHandler m_DoubleClickCommand;
        #endregion

        #region - Ctor -
        public TimeTableViewModel(Dispatcher Dispatcher, Station Station)
            : base()
        {
            this.Dispatcher = Dispatcher;
            CurrentStation = Station;
            Schedules = new ObservableCollection<TimeTableItemViewModel>();

            m_CalculationBll = new CalculationBLL();
            m_DoubleClickCommand = new CommandHandler(__RowDoubleClick, true);
        }
        #endregion

        #region - Properties -

        #region [Dispatcher]
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
        #endregion

        #region [Schedules]
        public ObservableCollection<TimeTableItemViewModel> Schedules
        {
            get;
            private set;
        }
        #endregion

        #region [CurrentStation]
        public Station CurrentStation
        {
            get;
            private set;
        }
        #endregion

        #region [Caption]
        public string Caption
        {
            get
            {
                return String.Format("Bahnhofsfahrordnung {0} ({1})", CurrentStation.Name, CurrentStation.RefNumber);
            }
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
        public TimeTableItemViewModel SelectedItem
        {
            get
            {
                return Get<TimeTableItemViewModel>();
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
            var CurrentSchedules = new List<TimeTableItemViewModel>();

            var SchedulesResult = m_CalculationBll.GetSchedulesByTime(CurrentStation.Schedules.Where(s => !s.IsUnscheduled), CurrentStation.ESTW.Time);

            if (!SchedulesResult.Succeeded)
            {
                Dispatcher.Invoke(() => MessageBox.Show(SchedulesResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
                return;
            }

            foreach (var Schedule in SchedulesResult.Result)
            {
                if (Schedule.Train == null)
                    continue;

                bool Visible = true;
                bool IsNew = false;

                if (Schedule.Time < CurrentStation.ESTW.Time)
                    Visible = false;

                var Current = Schedules.FirstOrDefault(s => s.TrainNumber == Schedule.Train.Number);

                if (Current == null)
                {
                    Current = new TimeTableItemViewModel(Schedule);
                    IsNew = true;
                }

                if (Area.LiveTrains.ContainsKey(Current.TrainNumber))
                {
                    var LiveTrain = Area.LiveTrains[Current.TrainNumber];
                    Current.Delay = LiveTrain.Delay;

                    var LiveSchedule = LiveTrain.Schedules.FirstOrDefault(s => s.Schedule == Schedule);

                    if (LiveSchedule != null)
                    {
                        Visible = true;
                        Current.LiveTrack = LiveSchedule.LiveTrack;

                        var Index = LiveTrain.Schedules.IndexOf(LiveSchedule);

                        if (LiveSchedule.LiveDeparture != null || LiveTrain.Schedules.Where((s, i) => i > Index && s.LiveArrival != null).Count() > 0)
                            Visible = false;

                        if (LiveSchedule.IsCancelled)
                            Visible = false;
                    }

                    LeibitTime ActualTime;

                    if (LiveTrain.Block != null && LiveTrain.Block.Track != null)
                        ActualTime = LiveTrain.Block.Track.Station.ESTW.Time;
                    else
                        ActualTime = Area.ESTWs.Where(e => e.IsLoaded).DefaultIfEmpty().Max(e => e.Time);

                    if (ActualTime != null && LiveTrain.LastModified != null && ActualTime > LiveTrain.LastModified.AddMinutes(1))
                        Visible = false;

                    if (Visible)
                    {
                        var DelayInfos = LiveTrain.Schedules.SelectMany(s => s.Delays).Where(d => d.Reason.IsNotNullOrWhiteSpace()).Select(d => d.Reason);

                        if (DelayInfos.Count() > 0)
                            Current.DelayReason = String.Join(", ", DelayInfos);

                        var StartSchedule = LiveTrain.Schedules.SingleOrDefault(s => s.Schedule.Handling == eHandling.Start && s.Schedule.Station == CurrentStation);
                        Current.IsReady = StartSchedule != null && StartSchedule.Schedule.Station.ESTW.Time >= StartSchedule.Schedule.Departure.AddMinutes(-2);
                    }
                }

                if (Visible)
                {
                    if (IsNew)
                        Dispatcher.Invoke(() => Schedules.Add(Current));

                    CurrentSchedules.Add(Current);
                }
            }

            var RemovedItems = Schedules.Except(CurrentSchedules).ToList();
            Dispatcher.Invoke(() => RemovedItems.ForEach(s => Schedules.Remove(s)));
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

            var Window = new TrainScheduleView(SelectedItem.TrainNumber);
            Window.DataContext = new TrainScheduleViewModel(Window.Dispatcher, SelectedItem.CurrentTrain, CurrentStation.ESTW.Area);
            OnOpenWindow(Window);
        }
        #endregion

        #endregion

    }
}
