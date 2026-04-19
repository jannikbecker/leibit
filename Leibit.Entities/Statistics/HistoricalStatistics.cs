using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using System.Collections.Generic;

namespace Leibit.Entities.Statistics
{
    public class HistoricalStatistics
    {
        public HistoricalStatistics()
        {
            TimeFrames = [];
        }

        public ESTW ESTW { get; set; }
        public int NumberOfTrains { get; set; }
        public TrainInformation TrainWithSmallestDelay { get; set; }
        public TrainInformation TrainWithGreatestDelay { get; set; }
        public double AverageDelay { get; set; }
        public int NumberOfTrainsOnTime { get; set; }
        public int NumberOfEarlyTrains { get; set; }
        public int NumberOfTrainsWithShortDelay { get; set; }
        public int NumberOfTrainsWithLongDelay { get; set; }
        public List<StatisticsTimeFrame> TimeFrames { get; set; }
    }
}
