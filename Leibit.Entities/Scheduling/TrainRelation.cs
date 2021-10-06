using Leibit.Core.Scheduling;
using System.Collections.Generic;

namespace Leibit.Entities.Scheduling
{
    public class TrainRelation
    {
        public TrainRelation(int trainNumber)
        {
            TrainNumber = trainNumber;
            Days = new List<eDaysOfService>();
        }

        public int TrainNumber { get; }
        public List<eDaysOfService> Days { get; }
    }
}
