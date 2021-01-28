using Leibit.Core.Scheduling;
using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedHiddenScheduleInfo
    {
        public int TrainNumber { get; set; }
        public string EstwId { get; set; }
        public string Station { get; set; }
        public LeibitTime Time { get; set; }
        public LeibitTime CreatedOn { get; set; }
    }
}
