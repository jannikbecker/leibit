using Leibit.Core.Scheduling;

namespace Leibit.Entities.LiveData
{
    public class Reminder
    {
        public int TrainNumber { get; set; }
        public string StationShort { get; set; }
        public LeibitTime DueTime { get; set; }
        public string Text { get; set; }
    }
}
