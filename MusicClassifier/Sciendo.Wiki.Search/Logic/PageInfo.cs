using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Logic
{
    public class PageInfo
    {
        [JsonProperty("sroffset")]
        public int Offset { get; set; }

        [JsonProperty("continue")]
        public string Continuation { get; set; }

    }
}
