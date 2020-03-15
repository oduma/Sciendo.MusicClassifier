using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class Transforms
    {
        public Dictionary<string, string> ArtistNamesMutation { get; set; }

        public Dictionary<string, string> LatinAlphabetTransformations { get; set; }

        public Dictionary<string, string> PersonalTitlesAssimilations { get; set; }

    }
}
