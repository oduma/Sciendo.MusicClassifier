using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class Spliters
    {
        public char WordsSimpleSplitter { get; set; }
        //public string FeaturedArtistsInTheTitle { get; set; }

        //splitter simple condition based on number of words before and after
        public Dictionary<string, int> ConditionalSplitters { get; set; }
    }
}
