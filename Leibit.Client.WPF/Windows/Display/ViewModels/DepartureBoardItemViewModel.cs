using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Scheduling;
using Leibit.Entities.Scheduling;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class DepartureBoardItemViewModel : ViewModelBase
    {

        #region - Ctor -
        public DepartureBoardItemViewModel(ScheduleItem scheduleItem)
        {
            Schedule = scheduleItem.Schedule;
        }
        #endregion

        #region - Properties -

        internal Schedule Schedule { get; }

        public LeibitTime Time => Schedule.Departure;

        public string TrainNumber
        {
            get => Get<string>();
            internal set => Set(value);
        }

        public string Via
        {
            get => Get<string>();
            internal set => Set(value);
        }

        public string Destination => Schedule.Train.Destination;

        public string TrackName
        {
            get => Get<string>();
            internal set => Set(value);
        }

        public string InfoText
        {
            get => Get<string>();
            internal set => Set(value);
        }

        #endregion

    }
}
