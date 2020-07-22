using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;

namespace Leibit.Client.WPF.Windows.TrainProgressInformation.ViewModels
{
    public class TrainStationViewModel : ViewModelBase
    {

        #region - Ctor -
        public TrainStationViewModel(TrainInformation Train, Schedule schedule)
        {
            CurrentTrain = Train;
            Schedule = schedule;
            Station = schedule.Station;
            TrainNumber = Train.Train.Number;
            Track = schedule.Track;

            Arrival = schedule.Arrival;
            Departure = schedule.Departure;

            if (Arrival == null)
            {
                Arrival = Departure;
                IsArrivalVisible = false;
            }
            else
                IsArrivalVisible = true;

            if (Departure == null)
            {
                Departure = Arrival;
                IsDepartureVisible = false;
            }
            else
                IsDepartureVisible = true;
        }
        #endregion

        #region - Properties -

        #region [CurrentTrain]
        public TrainInformation CurrentTrain
        {
            get;
            private set;
        }
        #endregion

        #region [Schedule]
        public Schedule Schedule
        {
            get;
            private set;
        }
        #endregion

        #region [Station]
        public Station Station
        {
            get
            {
                return Get<Station>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [TrainNumber]
        public int TrainNumber
        {
            get
            {
                return Get<int>();
            }
            set
            {
                Set(value);
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
            set
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
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsArrivalVisible]
        public bool IsArrivalVisible
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [IsDepartureVisible]
        public bool IsDepartureVisible
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [Delay]
        public int Delay
        {
            get
            {
                return Get<int>();
            }
            set
            {
                Set(value);
                OnPropertyChanged("DelayString");
            }
        }
        #endregion

        #region [DelayString]
        public string DelayString
        {
            get
            {
                return Delay > 0 ? String.Format("+{0}", Delay) : Delay.ToString();
            }
        }
        #endregion

        #region [ExpectedArrival]
        public LeibitTime ExpectedArrival
        {
            get
            {
                return Get<LeibitTime>();
            }
            set
            {
                if (value == null)
                {
                    Set(ExpectedDeparture);
                    IsExpectedArrivalVisible = false;
                }
                else
                {
                    Set(value);
                    IsExpectedArrivalVisible = true;
                }
            }
        }
        #endregion

        #region [ExpectedDeparture]
        public LeibitTime ExpectedDeparture
        {
            get
            {
                return Get<LeibitTime>();
            }
            set
            {
                if (value == null)
                {
                    Set(ExpectedArrival);
                    IsExpectedDepartureVisible = false;
                }
                else
                {
                    Set(value);
                    IsExpectedDepartureVisible = true;
                }
            }
        }
        #endregion

        #region [IsExpectedArrivalVisible]
        public bool IsExpectedArrivalVisible
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [IsExpectedDepartureVisible]
        public bool IsExpectedDepartureVisible
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [DelayInfo]
        public char DelayInfo
        {
            get
            {
                return Get<char>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [LocalOrders]
        public char LocalOrders
        {
            get => Get<char>();
            set => Set(value);
        }
        #endregion

        #region [CurrentStation]
        public Station CurrentStation
        {
            get
            {
                return Get<Station>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [State]
        public string State
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

        #region [Track]
        public Track Track
        {
            get
            {
                return Get<Track>();
            }
            set
            {
                Set(value);
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
            }
        }
        #endregion

        #endregion

    }
}
