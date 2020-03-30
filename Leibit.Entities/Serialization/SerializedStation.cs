using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedStation
    {
        public SerializedStation()
        {

        }

        public string EstwId { get; set; }
        public string ShortSymbol { get; set; }
    }
}
