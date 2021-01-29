using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedBlock
    {
        public string EstwId { get; set; }
        public string Name { get; set; }
        public eBlockDirection Direction { get; set; }
    }
}
