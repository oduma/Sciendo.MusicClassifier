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

        public Dictionary<string, Conditions> ConditionalSplitters { get; set; }
        public Dictionary<char, IEnumerable<ExceptionDefinition>> ConditionalWordsSplitters { get; set; }
    }
}
