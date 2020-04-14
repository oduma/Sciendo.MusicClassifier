using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using System.Collections.Generic;

namespace Sciendo.ArtistClassifier.NLP.NER.Logic
{
    public class PersonsNameFinder : BaseNLP, IPersonsNameFinder
    {
        public PersonsNameFinder(NlpConfig nlpConfig) : base(nlpConfig)
        {
        }

        public IEnumerable<string> FindPersonNames(string text)
        {
            return IsolatePersonNames(ParseToCrf(classifier, text));
        }
    }
}
