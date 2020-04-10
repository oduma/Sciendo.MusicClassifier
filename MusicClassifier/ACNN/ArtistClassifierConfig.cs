using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACNN
{
    public class ArtistClassifierConfig
    {
        [ConfigProperty("nlpConfig")]
        public NlpConfig NlpConfig { get; set; }
    }
}
