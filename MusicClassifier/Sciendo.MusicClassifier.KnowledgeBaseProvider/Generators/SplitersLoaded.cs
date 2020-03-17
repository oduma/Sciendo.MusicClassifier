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

        public Dictionary<string, Condition> ConditionalSplitters = new Dictionary<string, Condition>
        {
            {"+",new Condition{Length=2, Content=null } },
            {"&",new Condition{Length=2,Content=new []
            {
                "a ",
                "her ",
                "his ",
                "the ",
            } } },
            {"and",new Condition{Length=2,Content=new []
            {
                "a ",
                "her ",
                "his ",
                "the ",
            } } },
        };
    }
}
