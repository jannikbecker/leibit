using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.LiveData;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class DelayDetailViewModel : ViewModelBase
    {
        #region - Ctor -
        public DelayDetailViewModel(DelayInfo delay)
        {
            Train = delay.Schedule.Train;
            TrainType = delay.Schedule.Schedule.TrainType;
            TrainNumber = Train.Train.Number;
            DelayType = delay.Type == eDelayType.Arrival ? "Ankunft" : "Abfahrt";
            ScheduledArrival = delay.Schedule.Schedule.Arrival;
            LiveArrival = delay.Schedule.LiveArrival;
            ScheduledDeparture = delay.Schedule.Schedule.Departure;
            LiveDeparture = delay.Schedule.LiveDeparture;
            Delay = __FormatDelay(delay.Minutes);
            Reason = delay.Reason;
            CausedBy = delay.CausedBy;

            if (LiveArrival != null && ScheduledArrival != null)
                DelayArrival = __FormatDelay((LiveArrival - ScheduledArrival).TotalMinutes);
            if (LiveDeparture != null && ScheduledDeparture != null)
                DelayDeparture = __FormatDelay((LiveDeparture - ScheduledDeparture).TotalMinutes);
        }
        #endregion

        #region - Properties -

        #region [Train]
        public TrainInformation Train
        {
            get => Get<TrainInformation>();
            private set => Set(value);
        }
        #endregion

        #region [TrainType]
        public string TrainType
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [TrainNumber]
        public int TrainNumber
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [DelayType]
        public string DelayType
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [ScheduledArrival]
        public LeibitTime ScheduledArrival
        {
            get => Get<LeibitTime>();
            private set => Set(value);
        }
        #endregion

        #region [LiveArrival]
        public LeibitTime LiveArrival
        {
            get => Get<LeibitTime>();
            private set => Set(value);
        }
        #endregion

        #region [DelayArrival]
        public string DelayArrival
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [ScheduledDeparture]
        public LeibitTime ScheduledDeparture
        {
            get => Get<LeibitTime>();
            private set => Set(value);
        }
        #endregion

        #region [LiveDeparture]
        public LeibitTime LiveDeparture
        {
            get => Get<LeibitTime>();
            private set => Set(value);
        }
        #endregion

        #region [DelayDeparture]
        public string DelayDeparture
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [Delay]
        public string Delay
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [Reason]
        public string Reason
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [CausedBy]
        public int? CausedBy
        {
            get => Get<int?>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__FormatDelay]
        private string __FormatDelay(int delay)
        {
            if (delay < 0)
                return delay.ToString();
            else
                return $"+{delay}";
        }
        #endregion

        #endregion
    }
}
