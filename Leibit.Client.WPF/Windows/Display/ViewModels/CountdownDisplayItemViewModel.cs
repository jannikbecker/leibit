using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Scheduling;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class CountdownDisplayItemViewModel : ViewModelBase
    {

        #region - Ctor -
        public CountdownDisplayItemViewModel(ScheduleItem scheduleItem)
        {
            Schedule = scheduleItem.Schedule;
        }
        #endregion

        #region - Properties -

        #region [Schedule]
        internal Schedule Schedule { get; }
        #endregion

        #region [TrainNumber]
        public string TrainNumber
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

        #region [ExpectedDeparture]
        public string ExpectedDeparture
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #endregion

    }
}
