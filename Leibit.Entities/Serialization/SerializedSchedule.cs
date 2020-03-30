using Leibit.Core.Scheduling;
using System;
using System.Collections.Generic;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedSchedule
    {
        public SerializedSchedule()
        {
            Delays = new List<SerializedDelay>();
        }

        public string EstwId { get; set; }
        public string StationShortSymbol { get; set; }
        public LeibitTime StationTime { get; set; }
        public LeibitTime LiveArrival { get; set; }
        public LeibitTime LiveDeparture { get; set; }
        public string LiveTrack { get; set; }
        public LeibitTime ExpectedArrival { get; set; }
        public LeibitTime ExpectedDeparture { get; set; }
        public List<SerializedDelay> Delays { get; set; }
    }
}
