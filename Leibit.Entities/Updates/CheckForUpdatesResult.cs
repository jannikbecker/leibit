using System.Collections.Generic;

namespace Leibit.Entities.Updates
{
    public class CheckForUpdatesResult
    {
        public CheckForUpdatesResult()
        {
            ReleasesToApply = new List<ReleaseInfo>();
        }

        public string CurrentVersion { get; set; }
        public string FutureVersion { get; set; }
        public List<ReleaseInfo> ReleasesToApply { get; set; }
    }
}
