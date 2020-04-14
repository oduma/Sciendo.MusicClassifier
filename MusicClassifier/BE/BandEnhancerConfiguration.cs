using Sciendo.Config;
using Sciendo.Wiki.Search.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BE
{
    public class BandEnhancerConfiguration
    {
        [ConfigProperty("wikiSearch")]
        public WikiSearchConfig WikiSearchConfig { get; set; }

    }
}
