namespace Leibit.Entities.LiveData
{
    public class SharedDelay
    {
        public SharedDelay()
        {
        }

        public SharedDelay(int trainNumber, string stationShortSymbol, int scheduleIndex, int minutes, eDelayType type, string reason)
        {
            TrainNumber = trainNumber;
            StationShortSymbol = stationShortSymbol;
            ScheduleIndex = scheduleIndex;
            Minutes = minutes;
            Type = type;
            Reason = reason;
        }

        public int TrainNumber { get; set; }
        public string StationShortSymbol { get; set; }
        public int ScheduleIndex { get; set; }
        public int Minutes { get; set; }
        public eDelayType Type { get; set; }
        public string Reason { get; set; }
        public int? CausedBy { get; set; }
    }
}
