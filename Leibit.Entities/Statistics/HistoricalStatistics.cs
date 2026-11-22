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
            TrainsWithGreatestDelays = [];
            StationsWithGreatestDelays = [];
        }

        public ESTW ESTW { get; set; }
        public int NumberOfTrains { get; set; }
        public int? OnTimeRate { get; set; }
        public int NumberOfTrainsOnTime { get; set; }
        public int NumberOfEarlyTrains { get; set; }
        public int NumberOfTrainsWithShortDelay { get; set; }
        public int NumberOfTrainsWithLongDelay { get; set; }
        public int DelayReduced { get; set; }
        public int DelayAdded { get; set; }
        public int DelayTotalNet { get; set; }
        public List<StatisticsTimeFrame> TimeFrames { get; set; }
        public List<TrainInformation> TrainsWithGreatestDelays { get; set; }
        public List<StationDelayStatistic> StationsWithGreatestDelays { get; set; }
    }
}
