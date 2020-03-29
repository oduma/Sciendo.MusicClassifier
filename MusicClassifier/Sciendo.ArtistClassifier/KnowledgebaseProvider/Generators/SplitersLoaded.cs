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

        public Dictionary<string, Conditions> ConditionalSplitters = new Dictionary<string, Conditions>
        {
            //if in the artist there is a + split it if all parts are at least 2 words per part
            {"+",new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,
                        LengthAppliesToSplitParts=Applicability.All
                    }
                }
            },
            //if in the artist there is a : split it if all parts are at least 2 words per part
            {":",new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,
                        LengthAppliesToSplitParts=Applicability.All
                    }
                }
            },
            //if in the artist there is a & split it if all parts are at least 2 words per part,
            //except if any of the parts contains the words: a, her, his
            //or it starts with the word: the
            {"&",new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,
                        LengthAppliesToSplitParts=Applicability.All,
                        ExceptIfAnyPartsEqualRegex=new []
                        {
                            @"(?:^|\W)a(?:$|\W)",
                            @"(?:^|\W)her(?:$|\W)",
                            @"(?:^|\W)his(?:$|\W)",
                            @"^(?:^|\W)the(?:$|\W)",
                        }
                    }
                }
            },
            //if in the artist there is an 'and' split it if all parts are at least 2 words per part,
            //except if any of the parts contains the words: a, her, his, the
            {"and ",new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,
                        LengthAppliesToSplitParts=Applicability.All,
                        ExceptIfAnyPartsEqualRegex=new []
                        {
                            @"(?:^|\W)a(?:$|\W)",
                            @"(?:^|\W)her(?:$|\W)",
                            @"(?:^|\W)his(?:$|\W)",
                            @"^(?:^|\W)the(?:$|\W)",
                        }
                    }
                }
            },
            //if in the artist there is a word 'con' use it as a splitter point,
            //except if is the first word
            {"con ", new Conditions
                {
                    ExceptionPositionDefinition= new ExceptionDefinition
                    {
                        Position=Position.First
                    }
                } 
            },
            //if in the artist there is a word 'der' use it as a splitter point,
            //except if is the first word
            {"der ", new Conditions
                {
                    ExceptionPositionDefinition= new ExceptionDefinition
                    {
                        Position=Position.First
                    }
                } 
            },
            //if in the artist there is an 'feat' split it if all parts are at least 1 words per part,
            {" feat ", new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=1,LengthAppliesToSplitParts=Applicability.All
                    },
                }
            },
            //if in the artist there is an 'feat' split it if all parts are at least 1 words per part,
            {"feat. ", new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=1,LengthAppliesToSplitParts=Applicability.All
                    },
                }
            },
            //if in the artist there is an 'with' split it if all parts are at least 2 words per part,
            {" with ", new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,LengthAppliesToSplitParts=Applicability.All
                    },
                } 
            },
            //if in the artist there is an 'with' split it if any parts are at least 2 words per part,
            {" x ", new Conditions
                {
                    SplitPartsLengthCondition=new
                    SplitPartsLengthConditon
                    {
                        WordsPerPart=2,LengthAppliesToSplitParts=Applicability.Any
                    },
                } 
            },
        };

        public Dictionary<char, IEnumerable<ExceptionDefinition>> ConditionalWordsSplitters = 
            new Dictionary<char, IEnumerable<ExceptionDefinition>>
        {
            //split on ; always
            {';',null },
            //split on / allways
            {'/', null },
            //split on , except when the first part is numeric only 
            //or the last part is any of the following "etc.",".","!","?"
            {',', new []
                {
                    new ExceptionDefinition
                    {
                        Position=Position.First,
                        RegexTemplates=new []
                        {
                            @"^\d+$"
                        }
                    },
                    new ExceptionDefinition
                    {
                        Position=Position.Last,
                        RegexTemplates= new []
                        {
                            @"(?:^|\W)etc\.(?:$|\W)",
                            @"\.",
                            @"\!",
                            @"\?",
                        }
                    }
                }
            },
        };
    }
}
