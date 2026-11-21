using Leibit.Entities.LiveData;

namespace Leibit.Entities.Statistics
{
    public class TrainTypeStatistics
    {
        public int NumberOfTrains { get; set; }
        public int DelaySum { get; set; }
        public int? OnTimeRate { get; set; }
        public int NumberOfTrainsOnTime { get; set; }
        public int NumberOfEarlyTrains { get; set; }
        public int NumberOfTrainsWithShortDelay { get; set; }
        public int NumberOfTrainsWithLongDelay { get; set; }
        public TrainInformation TrainWithGreatestDelay { get; set; }
    }
}
