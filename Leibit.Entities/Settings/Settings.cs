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
        public bool CheckPlausibility { get; set; }
        public bool DisplayCompleteTrainSchedule { get; set; }
        public int EstwTimeout { get; set; }
        public bool LoadInactiveEstws { get; set; }
        public bool AutomaticReadyMessageEnabled { get; set; }
        public int AutomaticReadyMessageTime { get; set; }
        public string EstwOnlinePath { get; set; }
        public int? WindowColor { get; set; }
        public List<GridSetting> GridSettings { get; set; }
        public WindowSettings WindowSettings { get; set; }
        public int? LeadTime { get; set; }
        public int? FollowUpTime { get; set; }
        public bool? AutomaticallyCheckForUpdates { get; set; }
        public bool AutomaticallyInstallUpdates { get; set; }
        public string SkipVersion { get; set; }

        public Settings Clone()
        {
            var Result = new Settings();

            Result.Paths = new Dictionary<string, string>(this.Paths);
            Result.DelayJustificationEnabled = this.DelayJustificationEnabled;
            Result.DelayJustificationMinutes = this.DelayJustificationMinutes;
            Result.CheckPlausibility = this.CheckPlausibility;
            Result.DisplayCompleteTrainSchedule = this.DisplayCompleteTrainSchedule;
            Result.EstwTimeout = this.EstwTimeout;
            Result.LoadInactiveEstws = this.LoadInactiveEstws;
            Result.AutomaticReadyMessageEnabled = this.AutomaticReadyMessageEnabled;
            Result.AutomaticReadyMessageTime = this.AutomaticReadyMessageTime;
            Result.EstwOnlinePath = this.EstwOnlinePath;
            Result.WindowColor = this.WindowColor;
            Result.LeadTime = this.LeadTime;
            Result.FollowUpTime = this.FollowUpTime;
            Result.AutomaticallyCheckForUpdates = this.AutomaticallyCheckForUpdates;
            Result.AutomaticallyInstallUpdates = this.AutomaticallyInstallUpdates;
            Result.SkipVersion = this.SkipVersion;
            Result.GridSettings = this.GridSettings; // No deep clone required
            Result.WindowSettings = this.WindowSettings; // No deep clone required

            return Result;
        }

    }
}
