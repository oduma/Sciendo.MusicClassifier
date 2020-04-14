using Newtonsoft.Json;

namespace Sciendo.Wiki.Search.Contracts.DataTypes
{
    public class SearchResult
    {
        [JsonProperty("ns")]
        public int Namespace { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("pageid")]
        public long PageId { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("wordcount")]
        public int WordCount { get; set; }
        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }

        public string Language { get; set; }

    }
}