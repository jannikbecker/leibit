using Leibit.Core.Scheduling;
using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedReminder
    {
        public int TrainNumber { get; set; }
        public string StationShort { get; set; }
        public LeibitTime DueTime { get; set; }
        public string Text { get; set; }
    }
}
