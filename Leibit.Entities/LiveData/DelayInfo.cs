
namespace Leibit.Entities.LiveData
{
    public class DelayInfo
    {
        internal DelayInfo(LiveSchedule schedule, int minutes, eDelayType type)
        {
            Schedule = schedule;
            Minutes = minutes;
            Type = type;
        }

        public LiveSchedule Schedule { get; private set; }
        public eDelayType Type { get; private set; }
        public int Minutes { get; private set; }
        public string Reason { get; set; }
        public int? CausedBy { get; set; }

        public DelayInfo Clone()
        {
            return new DelayInfo(Schedule, Minutes, Type);
        }
    }
}
