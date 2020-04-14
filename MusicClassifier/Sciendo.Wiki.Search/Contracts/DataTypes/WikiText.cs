using Newtonsoft.Json;

namespace Sciendo.Wiki.Search.Contracts.DataTypes
{
    public class WikiText
    {
        [JsonProperty("*")]
        public string AllText { get; set; }
    }
}