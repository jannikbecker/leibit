namespace Leibit.Entities.Common
{
    public class TrainNumberInfo
    {
        public TrainNumberInfo(string type, string pattern, string line)
        {
            Type = type;
            Pattern = pattern;
            Line = line;
        }

        public string Type { get; }
        public string Pattern { get; }
        public string Line { get; }
    }
}
