using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.Config;
using Sciendo.Wiki.Search.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AE
{
    public class BandEnhancerConfiguration
    {
        [ConfigProperty("wikiSearch")]
        public WikiSearchConfig WikiSearchConfig { get; set; }

        [ConfigProperty("nlpConfig")]
        public NlpConfig NlpConfig { get; set; }

        [ConfigProperty("knowledgeBaseFile")]
        public string KnowledgeBaseFile { get; set; }


    }
}
