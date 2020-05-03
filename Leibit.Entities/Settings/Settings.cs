using System.Collections.Generic;

namespace Leibit.Entities.Settings
{
    public class Settings
    {

        public Settings()
        {
            Paths = new Dictionary<string, string>();
            GridSettings = new List<GridSetting>();
        }

        public Dictionary<string, string> Paths { get; private set; }
        public bool DelayJustificationEnabled { get; set; }
        public int DelayJustificationMinutes { get; set; }
        public bool WriteDelayJustificationFile { get; set; }
        public string EstwOnlinePath { get; set; }
        public int? WindowColor { get; set; }
        public List<GridSetting> GridSettings { get; set; }

        public Settings Clone()
        {
            var Result = new Settings();

            Result.Paths = new Dictionary<string, string>(this.Paths);
            Result.DelayJustificationEnabled = this.DelayJustificationEnabled;
            Result.DelayJustificationMinutes = this.DelayJustificationMinutes;
            Result.WriteDelayJustificationFile = this.WriteDelayJustificationFile;
            Result.EstwOnlinePath = this.EstwOnlinePath;
            Result.WindowColor = this.WindowColor;
            Result.GridSettings = this.GridSettings; // No deep clone required

            return Result;
        }

    }
}
