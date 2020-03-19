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
            {"+",new Condition
                {
                    LengthConditions=new
                    LengthConditons
                    {
                        Length=2,AppliedTo=Applicability.Both
                    }
                }
            },
            {":",new Condition
                {
                    LengthConditions=new
                    LengthConditons
                    {
                        Length=2,AppliedTo=Applicability.Both
                    }
                }
            },
            {"&",new Condition
                {
                    LengthConditions=new
                    LengthConditons
                    {
                        Length=2,AppliedTo=Applicability.Both
                    },
                    NonSplittingContent=new []
                    {
                        "a ",
                        "her ",
                        "his ",
                        "the ",
                    } 
                } 
            },
            {"and",new Condition
                {
                    LengthConditions=new
                        LengthConditons
                        {
                            Length=2,AppliedTo=Applicability.Both
                        },
                    NonSplittingContent=new []
                    {
                        "a ",
                        "her ",
                        "his ",
                        "the ",
                    } 
                } 
            },
            {"con", new Condition{Position=0} },
            {"der", new Condition{Position=0} },
            {"with", new Condition
                {
                    LengthConditions=new
                    LengthConditons
                    {
                        Length=2,AppliedTo=Applicability.Both
                    },
                } 
            },
            {"x", new Condition
                {
                    LengthConditions=new
                    LengthConditons
                    {
                        Length=2,AppliedTo=Applicability.Any
                    },
                } 
            },
        };

        public Dictionary<char, Condition> ConditionalWordsSplitters = new Dictionary<char, Condition>
        {
            {' ', null },
            {';',null },
            {'/', null },
            {',', null },
        };
    }
}
