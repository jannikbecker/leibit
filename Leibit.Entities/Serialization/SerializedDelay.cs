using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedDelay
    {
        public eDelayType Type { get; set; }
        public int Minutes { get; set; }
        public string Reason { get; set; }
        public int? CausedBy { get; set; }
    }
}
