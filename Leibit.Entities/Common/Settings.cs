﻿using System.Collections.Generic;

namespace Leibit.Entities.Common
{
    public class Settings
    {

        public Settings()
        {
            Paths = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Paths { get; private set; }
        public string EstwOnlinePath { get; set; }
        public int? WindowColor { get; set; }

        public Settings Clone()
        {
            var Result = new Settings();

            Result.Paths = new Dictionary<string, string>(this.Paths);
            Result.EstwOnlinePath = this.EstwOnlinePath;
            Result.WindowColor = this.WindowColor;

            return Result;
        }

    }
}
