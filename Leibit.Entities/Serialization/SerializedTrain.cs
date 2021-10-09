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
            BlockHistory = new List<SerializedBlock>();
        }

        public int TrainNumber { get; set; }
        public string CurrentEstwId { get; set; }
        public eBlockDirection TrainDirection { get; set; }
        public string Block { get; set; }
        public List<SerializedBlock> BlockHistory { get; set; }
        public eBlockDirection BlockDirection { get; set; }
        public int Delay { get; set; }
        public LeibitTime LastModified { get; set; }
        public LeibitTime CreatedOn { get; set; }
        public List<SerializedSchedule> Schedules { get; set; }
        public bool IsDestinationStationCancelled { get; set; }
    }
}
