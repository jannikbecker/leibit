using Leibit.Entities.Common;
using System.Collections.Generic;

namespace Leibit.Entities.Serialization
{
    public class SerializationContainer
    {

        public SerializationContainer()
        {
            Windows = new List<SerializedWindowInformation>();
            VisibleStations = new List<SerializedStation>();
        }

        public Area Area { get; set; }
        public List<SerializedWindowInformation> Windows { get; set; }
        public List<SerializedStation> VisibleStations { get; set; }
        public List<SerializedVisibleTrainInfo> VisibleTrains { get; set; }
        public List<SerializedHiddenScheduleInfo> HiddenSchedules { get; set; }
        public bool IsOldVersion { get; set; }
        public eFileFormat FileFormat { get; set; }

    }
}
