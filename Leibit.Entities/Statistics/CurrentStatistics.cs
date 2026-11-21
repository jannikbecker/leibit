using Leibit.Entities.Common;

namespace Leibit.Entities.Statistics
{
    public class CurrentStatistics
    {
        public ESTW ESTW { get; set; }

        public TrainTypeStatistics Total { get; set; }
        public TrainTypeStatistics PassengerTrains { get; set; }
        public TrainTypeStatistics FreightTrains { get; set; }
    }
}
