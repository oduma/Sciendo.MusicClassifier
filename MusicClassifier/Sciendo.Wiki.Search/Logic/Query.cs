using Newtonsoft.Json;
using Sciendo.Wiki.Search.Contracts.DataTypes;

namespace Sciendo.Wiki.Search.Logic
{
    public class Query
    {
        [JsonProperty("searchinfo")]
        public SearchInfo SearchInfo { get; set; }

        [JsonProperty("search")]
        public SearchResult[] SearchResults { get; set; }
    }
}