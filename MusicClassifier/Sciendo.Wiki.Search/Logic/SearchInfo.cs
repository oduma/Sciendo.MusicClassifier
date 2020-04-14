using Newtonsoft.Json;

namespace Sciendo.Wiki.Search.Logic
{
    public class SearchInfo
    {
        [JsonProperty("totalhits")]
        public int TotalHits { get; set; }
    }
}