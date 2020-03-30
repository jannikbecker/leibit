using Leibit.Core.Scheduling;
using System;
using System.Collections.Generic;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedTrain
    {
        public SerializedTrain()
        {
            Schedules = new List<SerializedSchedule>();
        }

        public int TrainNumber { get; set; }
        public string CurrentEstwId { get; set; }
        public string Block { get; set; }
        public eBlockDirection BlockDirection { get; set; }
        public int Delay { get; set; }
        public LeibitTime LastModified { get; set; }
        public List<SerializedSchedule> Schedules { get; set; }
    }
}
