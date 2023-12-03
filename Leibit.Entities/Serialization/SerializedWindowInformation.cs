﻿using Leibit.Core.Serialization;
using Newtonsoft.Json;
using System;

namespace Leibit.Entities.Serialization
{
    [Serializable]
    public class SerializedWindowInformation
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public eChildWindowType Type { get; set; }
        [JsonConverter(typeof(UntypedPropertyJsonConverter))]
        public object Tag { get; set; }
        public bool IsDockedOut { get; set; }
    }
}
