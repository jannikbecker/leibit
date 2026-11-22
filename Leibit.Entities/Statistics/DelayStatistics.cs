namespace Leibit.Entities.Statistics
{
    public class DelayStatistics
    {
        public int NumberOfTrainsOnTime { get; set; }
        public int NumberOfEarlyTrains { get; set; }
        public int NumberOfTrainsWithShortDelay { get; set; }
        public int NumberOfTrainsWithLongDelay { get; set; }
    }
}
