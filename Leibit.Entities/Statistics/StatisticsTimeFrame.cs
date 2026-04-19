using Leibit.Core.Scheduling;

namespace Leibit.Entities.Statistics
{
    public class StatisticsTimeFrame
    {
        public LeibitTime StartTime { get; set; }
        public LeibitTime EndTime { get; set; }
        public int NumberOfTrains { get; set; }
        public double AverageDelay { get; set; }
    }
}
