using System.Collections.Generic;

namespace Leibit.Entities.Common
{
    public class ScheduleFile
    {

        public ScheduleFile(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public List<string> Tracks { get; set; }

    }
}
