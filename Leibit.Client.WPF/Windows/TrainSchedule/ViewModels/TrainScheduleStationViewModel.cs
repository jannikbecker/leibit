using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.TrainSchedule.ViewModels
{
    public class TrainScheduleStationViewModel : ViewModelBase
    {

        #region - Needs -
        private CommandHandler m_DelayJustificationCommand;
        private CommandHandler m_LocalOrdersCommand;
        #endregion

        #region - Ctor -
        public TrainScheduleStationViewModel()
            : base()
        {
            IsFirstStation = true;
            IsLastStation = true;
        }

        public TrainScheduleStationViewModel(string StationName, bool IsDestination)
            : base()
        {
            this.StationName = StationName;
            IsFirstStation = !IsDestination;
            IsLastStation = IsDestination;
        }

        public TrainScheduleStationViewModel(Schedule Schedule)
            : base()
        {
            CurrentSchedule = Schedule;
            StationName = String.Format("{0} ({1})", Schedule.Station.Name, Schedule.Station.RefNumber);
            Arrival = Schedule.Arrival;
            Departure = Schedule.Departure;
            Track = Schedule.Track;
            Remark = Schedule.Remark;

            IsFirstStation = Schedule.Handling == eHandling.Start;
            IsLastStation = Schedule.Handling == eHandling.Destination;

            m_DelayJustificationCommand = new CommandHandler(__JustifyDelay, false);
            m_LocalOrdersCommand = new CommandHandler(__ShowLocalOrders, true);
        }
        #endregion

        #region - Properties -

        #region [CurrentSchedule]
        public Schedule CurrentSchedule
        {
            get
            {
                return Get<Schedule>();
            }
            private set
            {
                Set(value);
                OnPropertyChanged("HasSchedule");
            }
        }
        #endregion

        #region [StationName]
        public string StationName
        {
            get
            {
                return Get<string>();
            }
            private set
            {
                Set(value);
                OnPropertyChanged("HasStation");
            }
        }
        #endregion

        #region [Arrival]
        public LeibitTime Arrival
        {
            get
            {
                return Get<LeibitTime>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [Departure]
        public LeibitTime Departure
        {
            get
            {
                return Get<LeibitTime>();
            }
            internal set
            {
                Set(value);
                OnPropertyChanged();
            }
        }
        #endregion

        #region [DelayArrival]
        public int? DelayArrival
        {
            get
            {
                return Get<int?>();
            }
            set
            {
                Set(value);
                OnPropertyChanged("DelayArrivalString");
            }
        }
        #endregion

        #region [DelayDeparture]
        public int? DelayDeparture
        {
            get
            {
                return Get<int?>();
            }
            set
            {
                Set(value);
                OnPropertyChanged("DelayDepartureString");
            }
        }
        #endregion

        #region [DelayArrivalString]
        public string DelayArrivalString
        {
            get
            {
                if (DelayArrival.HasValue)
                    return DelayArrival >= 0 ? String.Format("+{0}", DelayArrival) : DelayArrival.ToString();

                return null;
            }
        }
        #endregion

        #region [DelayDepartureString]
        public string DelayDepartureString
        {
            get
            {
                if (DelayDeparture.HasValue)
                    return DelayDeparture >= 0 ? String.Format("+{0}", DelayDeparture) : DelayDeparture.ToString();

                return null;
            }
        }
        #endregion

        #region [ArrivalVisibility]
        public Visibility ArrivalVisibility
        {
            get
            {
                return Arrival != null && !CurrentSchedule.IsUnscheduled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region [DepartureVisibility]
        public Visibility DepartureVisibility
        {
            get
            {
                return Departure != null && !CurrentSchedule.IsUnscheduled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region [LiveTime]
        public LeibitTime LiveTime
        {
            get
            {
                if (Departure == null)
                {
                    if (DelayArrival.HasValue)
                        return Arrival.AddMinutes(DelayArrival.Value);
                }
                else
                {
                    if (DelayDeparture.HasValue)
                        return Departure.AddMinutes(DelayDeparture.Value);
                }

                return null;
            }
        }
        #endregion

        #region [Track]
        public Track Track
        {
            get
            {
                return Get<Track>();
            }
            private set
            {
                Set(value);
                OnPropertyChanged("TrackName");
            }
        }
        #endregion

        #region [LiveTrack]
        public Track LiveTrack
        {
            get
            {
                return Get<Track>();
            }
            set
            {
                Set(value);
                OnPropertyChanged("TrackName");
            }
        }
        #endregion

        #region [TrackName]
        public string TrackName
        {
            get
            {
                if (Track == null && LiveTrack == null)
                    return null;

                if (Track != null && LiveTrack == null)
                    return Track.Name;

                if (Track == null && LiveTrack != null)
                    return LiveTrack.Name;

                if (Track.Name == LiveTrack.Name)
                    return Track.Name;

                return String.Format("{0} ({1})", LiveTrack.Name, Track.Name);
            }
        }
        #endregion

        #region [HasSchedule]
        public bool HasSchedule
        {
            get
            {
                return CurrentSchedule != null;
            }
        }
        #endregion

        #region [HasStation]
        public bool HasStation
        {
            get
            {
                return StationName.IsNotNullOrWhiteSpace();
            }
        }
        #endregion

        #region [StationDotVisibility]
        public Visibility StationDotVisibility
        {
            get
            {
                if (!HasStation)
                    return Visibility.Collapsed;

                if (IsFirstStation || IsLastStation)
                    return Visibility.Visible;

                return IsSkipped ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region [IsFirstStation]
        public bool IsFirstStation
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsLastStation]
        public bool IsLastStation
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsArrived]
        public bool IsArrived
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
                OnPropertyChanged(nameof(ArrivalColor));
            }
        }
        #endregion

        #region [IsDeparted]
        public bool IsDeparted
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
                OnPropertyChanged(nameof(DepartureColor));
            }
        }
        #endregion

        #region [IsSkipped]
        public bool IsSkipped
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
                OnPropertyChanged(nameof(ArrivalColor));
                OnPropertyChanged(nameof(DepartureColor));
                OnPropertyChanged(nameof(TextColor));
                OnPropertyChanged(nameof(StationDotVisibility));
            }
        }
        #endregion

        #region [ArrivalColor]
        public Brush ArrivalColor
        {
            get
            {
                if (IsSkipped)
                    return Brushes.LightCoral;

                return IsArrived ? Brushes.Red : App.Current.Resources["TrainScheduleLineColor"] as Brush;
            }
        }
        #endregion

        #region [DepartureColor]
        public Brush DepartureColor
        {
            get
            {
                if (IsSkipped)
                    return Brushes.LightCoral;

                return IsDeparted ? Brushes.Red : App.Current.Resources["TrainScheduleLineColor"] as Brush;
            }
        }
        #endregion

        #region [TextColor]
        public Brush TextColor
        {
            get
            {
                return IsSkipped ? Brushes.Gray : App.Current.Resources["TextForeground"] as Brush;
            }
        }
        #endregion

        #region [Remark]
        public string Remark
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [HasRemark]
        public bool HasRemark
        {
            get
            {
                return Remark.IsNotNullOrWhiteSpace();
            }
        }
        #endregion

        #region [DelayInfo]
        public string DelayInfo
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsDelayJustified]
        public bool IsDelayJustified
        {
            set
            {
                m_DelayJustificationCommand.SetCanExecute(!value);
            }
        }
        #endregion

        #region [DelayJustificationCommand]
        public ICommand DelayJustificationCommand
        {
            get
            {
                return m_DelayJustificationCommand;
            }
        }
        #endregion

        #region [LocalOrdersCommand]
        public ICommand LocalOrdersCommand
        {
            get
            {
                return m_LocalOrdersCommand;
            }
        }
        #endregion

        #region [HasLocalOrders]
        public bool HasLocalOrders
        {
            get
            {
                if (CurrentSchedule == null)
                    return false;

                return CurrentSchedule.LocalOrders.IsNotNullOrWhiteSpace();
            }
        }
        #endregion

        #region [IsDelayArrivalManuallySet]
        public bool IsDelayArrivalManuallySet
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [IsDelayDepartureManuallySet]
        public bool IsDelayDepartureManuallySet
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #endregion

        #region - Internal methods -

        #region [SkinChanged]
        internal void SkinChanged()
        {
            OnPropertyChanged(nameof(ArrivalColor));
            OnPropertyChanged(nameof(DepartureColor));
            OnPropertyChanged(nameof(TextColor));
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__JustifyDelay]
        private void __JustifyDelay()
        {
            OnPropertyChanged("JustifyDelay");
        }
        #endregion

        #region [__ShowLocalOrders]
        private void __ShowLocalOrders()
        {
            OnPropertyChanged("ShowLocalOrders");
        }
        #endregion

        #endregion

    }
}
