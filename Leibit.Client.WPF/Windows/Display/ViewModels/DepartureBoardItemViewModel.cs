using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities.Scheduling;
using System.Windows;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class DepartureBoardItemViewModel : ViewModelBase
    {

        #region - Needs -
        private bool m_ArrivalBoard;
        #endregion

        #region - Ctor -
        public DepartureBoardItemViewModel(ScheduleItem scheduleItem, bool arrivalBoard)
        {
            Schedule = scheduleItem.Schedule;
            m_ArrivalBoard = arrivalBoard;
        }
        #endregion

        #region - Properties -

        #region [Schedule]
        internal Schedule Schedule { get; }
        #endregion

        #region [Time]
        public LeibitTime Time => m_ArrivalBoard ? Schedule.Arrival : Schedule.Departure;
        #endregion

        #region [Margin]
        public Thickness Margin
        {
            get => Get<Thickness>();
            internal set => Set(value);
        }
        #endregion

        #region [TrainNumber]
        public string TrainNumber
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #region [IsTrackChanged]
        public bool IsTrackChanged
        {
            get => Get<bool>();
            internal set => Set(value);
        }
        #endregion

        #region [Via]
        public string Via
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #region [Destination]
        public string Destination
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #region [TrackName]
        public string TrackName
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #region [InfoText]
        public string InfoText
        {
            get => Get<string>();
            internal set
            {
                Set(value);
                OnPropertyChanged(nameof(IsInfoTextVisible));
            }
        }
        #endregion

        #region [IsInfoTextVisible]
        public bool IsInfoTextVisible => InfoText.IsNotNullOrWhiteSpace();
        #endregion

        #endregion

    }
}
