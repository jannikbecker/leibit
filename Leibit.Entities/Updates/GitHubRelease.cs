using Newtonsoft.Json;
using System;

namespace Leibit.Entities.Updates
{
    public class GitHubRelease
    {
        [JsonProperty("prerelease")]
        public bool PreRelease { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
    }
}
