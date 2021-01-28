using System;
using System.Collections.Generic;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedRoot
    {
        public SerializedRoot()
        {
            LiveTrains = new List<SerializedTrain>();
            Windows = new List<SerializedWindowInformation>();
            LoadedESTWs = new List<SerializedESTW>();
            VisibleStations = new List<SerializedStation>();
        }

        public List<SerializedTrain> LiveTrains { get; set; }
        public List<SerializedWindowInformation> Windows { get; set; }
        public string AreaId { get; set; }
        public List<SerializedESTW> LoadedESTWs { get; set; }
        public List<SerializedStation> VisibleStations { get; set; }
        public List<SerializedVisibleTrainInfo> VisibleTrains { get; set; }
        public List<SerializedHiddenScheduleInfo> HiddenSchedules { get; set; }
    }
}
