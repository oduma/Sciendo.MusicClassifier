using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Logic
{
    public class WikiSearchResults
    {
        [JsonProperty("batchcomplete")]
        public string BatchComplete { get; set; }
        [JsonProperty("continue")]
        public PageInfo PageInfo { get; set; }
        [JsonProperty("query")]
        public Query Query { get; set; }

    }
}
