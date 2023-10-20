using Leibit.Core.Scheduling;
using System;
using System.Collections.Generic;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedESTW
    {
        public string ESTWId { get; set; }
        public LeibitTime Time { get; set; }
        public LeibitTime StartTime { get; set; }
        public bool IsActive { get; set; }
        public List<SerializedReminder> Reminders { get; set; }
    }
}
