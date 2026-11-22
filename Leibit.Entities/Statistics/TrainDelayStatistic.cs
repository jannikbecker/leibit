using Leibit.Entities.LiveData;

namespace Leibit.Entities.Statistics
{
    public class TrainDelayStatistic
    {
        public TrainInformation Train { get; set; }
        public int EntryDelay { get; set; }
        public int ExitDelay { get; set; }
        public int NetDelay { get; set; }
    }
}
