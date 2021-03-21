namespace Leibit.Entities.LiveData
{
    public class RefreshLiveDataResult
    {
        public string EstwId { get; set; }
        public bool Succeeded { get; set; }
        public bool HasRefreshed { get; set; }
        public string Message { get; set; }
    }
}
