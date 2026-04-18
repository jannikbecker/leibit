using Leibit.Entities.Common;
using Leibit.Entities.LiveData;

namespace Leibit.Entities.Statistics
{
    public class CurrentStatistics
    {
        public ESTW ESTW { get; set; }
        public int NumberOfTrains { get; set; }
        public TrainInformation TrainWithSmallestDelay { get; set; }
        public TrainInformation TrainWithGreatestDelay { get; set; }
        public double AverageDelay { get; set; }
        public int NumberOfTrainsOnTime { get; set; }
        public int NumberOfEarlyTrains { get; set; }
        public int NumberOfTrainsWithShortDelay { get; set; }
        public int NumberOfTrainsWithLongDelay { get; set; }
    }
}
