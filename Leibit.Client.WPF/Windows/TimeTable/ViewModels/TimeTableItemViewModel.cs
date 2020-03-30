using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System;

namespace Leibit.Client.WPF.Windows.TimeTable.ViewModels
{
    public class TimeTableItemViewModel : ViewModelBase
    {

        #region - Ctor -
        public TimeTableItemViewModel(Train Train)
        {
            CurrentTrain = Train;
        }
        #endregion

        #region - Properties -

        #region [CurrentTrain]
        public Train CurrentTrain
        {
            get;
            private set;
        }
        #endregion

        #region [Type]
        public string Type
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

        #region [Start]
        public string Start
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

        #region [Destination]
        public string Destination
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

        #region [Delay]
        public int? Delay
        {
            get
            {
                return Get<int?>();
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
                if (Delay.HasValue)
                    return Delay > 0 ? String.Format("+{0}", Delay) : Delay.ToString();

                return null;
            }
        }
        #endregion

        #region [DelayReason]
        public string DelayReason
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

        #region [IsReady]
        public bool IsReady
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

        #endregion

    }
}
