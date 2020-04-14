using Newtonsoft.Json;
using Sciendo.Wiki.Search.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Logic
{
    public class WikiPageText
    {
        [JsonProperty("parse")]
        public ParsedPage ParsedPage { get; set; }

    }
}
