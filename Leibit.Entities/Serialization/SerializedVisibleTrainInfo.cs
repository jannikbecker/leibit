using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedVisibleTrainInfo
    {
        public int TrainNumber { get; set; }
        public bool HadLiveData { get; set; }
    }
}
