using System.Collections.Generic;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    //x
    public class Conditions
    {
        public SplitPartsLengthConditon SplitPartsLengthCondition { get; set; }

        public ExceptionDefinition[] ExceptionPositionDefinitions { get; set; }
    }
}