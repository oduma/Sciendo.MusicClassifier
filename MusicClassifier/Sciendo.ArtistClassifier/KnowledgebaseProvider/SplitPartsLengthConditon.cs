using System.Collections.Generic;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    //x
    public class SplitPartsLengthConditon
    {
        public int? WordsPerPart { get; set; }

        public Applicability LengthAppliesToSplitParts { get; set; }

        public IEnumerable<string> ExceptIfAnyPartsEqualRegex { get; set; }

    }
}