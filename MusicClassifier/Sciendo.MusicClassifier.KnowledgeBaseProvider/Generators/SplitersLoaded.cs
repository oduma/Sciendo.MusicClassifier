using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class SplitersLoaded
    {
        public char WordsSimpleSplitter = ' ';
        public string FeaturedArtistsInTheTitle = @"\([^)]*\)";

        public Dictionary<string, int> ConditionalSplitters = new Dictionary<string, int>
        {
            {"+",2 }
        };
    }
}
