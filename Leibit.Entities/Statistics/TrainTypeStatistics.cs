using Leibit.Entities.LiveData;

namespace Leibit.Entities.Statistics
{
    public class TrainTypeStatistics
    {
        public TrainTypeStatistics()
        {
            DelayStatistics = new DelayStatistics();
        }

        public int NumberOfTrains { get; set; }
        public int DelaySum { get; set; }
        public DelayStatistics DelayStatistics { get; set; }
        public TrainInformation TrainWithGreatestDelay { get; set; }
    }
}
