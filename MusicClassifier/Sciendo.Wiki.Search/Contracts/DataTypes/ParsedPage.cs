using Newtonsoft.Json;

namespace Sciendo.Wiki.Search.Contracts.DataTypes
{
    public class ParsedPage
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("pageid")]
        public long PageId { get; set; }

        [JsonProperty("wikitext")]
        public WikiText WikiText { get; set; }
    }
}