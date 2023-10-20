using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Reminder.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {

        #region - Needs -
        private readonly LiveSchedule m_LiveSchedule;
        #endregion

        #region - Ctor -
        public ScheduleViewModel(TrainInformation train, Schedule schedule)
        {
            CurrentSchedule = schedule;
            m_LiveSchedule = train?.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == schedule.Station.ShortSymbol && s.Schedule.Time == schedule.Time);

            ShowLocalOrdersCommand = new CommandHandler(__ShowLocalOrders, HasLocalOrders);
        }
        #endregion

        #region - Properties -

        public Schedule CurrentSchedule { get; }

        #region [HasLiveData]
        public bool HasLiveData => m_LiveSchedule != null;
        #endregion

        #region [Arrival]
        public string Arrival => CurrentSchedule?.Arrival?.ToString() ?? "-";
        #endregion

        #region [DelayArrival]
        public string DelayArrival
        {
            get
            {
                if (CurrentSchedule.Arrival == null || m_LiveSchedule?.ExpectedArrival == null)
                    return null;

                var delay = (m_LiveSchedule.ExpectedArrival - CurrentSchedule.Arrival).TotalMinutes;

                return delay >= 0 ? $"+{delay}" : delay.ToString();
            }
        }
        #endregion

        #region [Departure]
        public string Departure => CurrentSchedule?.Departure?.ToString() ?? "-";
        #endregion

        #region [DelayDeparture]
        public string DelayDeparture
        {
            get
            {
                if (CurrentSchedule.Departure == null || m_LiveSchedule?.ExpectedDeparture == null)
                    return null;

                var delay = (m_LiveSchedule.ExpectedDeparture - CurrentSchedule.Departure).TotalMinutes;

                return delay >= 0 ? $"+{delay}" : delay.ToString();
            }
        }
        #endregion

        #region [Track]
        public string Track
        {
            get
            {
                var liveTrack = m_LiveSchedule?.LiveTrack?.Name;

                if (liveTrack == null || liveTrack == CurrentSchedule.Track.Name)
                    return CurrentSchedule.Track.Name;
                else
                    return $"{liveTrack} ({CurrentSchedule.Track.Name})";
            }
        }
        #endregion

        #region [Remark]
        public string Remark => CurrentSchedule.Remark;
        #endregion

        #region [HasLocalOrders]
        public bool HasLocalOrders => CurrentSchedule.LocalOrders.IsNotNullOrWhiteSpace();
        #endregion

        public CommandHandler ShowLocalOrdersCommand { get; }

        #endregion

        #region - Private methods -

        #region [__ShowLocalOrders]
        private void __ShowLocalOrders()
        {
            OnPropertyChanged("ShowLocalOrders");
        }
        #endregion

        #endregion

    }
}
