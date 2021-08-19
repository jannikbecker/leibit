using Leibit.Core.Scheduling;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    internal class ScheduleItem
    {

        internal ScheduleItem(LeibitTime referenceTime, Schedule schedule, LiveSchedule liveSchedule = null)
        {
            ReferenceTime = referenceTime;
            Schedule = schedule;
            LiveSchedule = liveSchedule;
        }

        internal LeibitTime ReferenceTime { get; }
        internal Schedule Schedule { get; }
        internal LiveSchedule LiveSchedule { get; }

    }
}
