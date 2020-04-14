using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using Sciendo.ArtistClassifier.NLP.NER.Contracts.DataTypes;
using System.Linq;

namespace Sciendo.ArtistClassifier.NLP.NER.Logic
{
    public class ArtistClassifier : BaseNLP, IArtistClassifier
    {


        public ArtistClassifier(NlpConfig nlpConfig):base(nlpConfig)
        {
        }
        public Artist Classify(string input)
        {
            var crfWithXml = ParseToCrf(classifier, input.ToUpper());
            var personNames = IsolatePersonNames(crfWithXml);
            if(personNames.Count()!=1)
            {
                return new Artist { Name = input, ArtistType = ArtistType.Band };
            }
            if(personNames.First()==input.ToUpper())
            {
                return new Artist { Name = input, ArtistType = ArtistType.Artist };
            }
            return new Artist { Name = input, ArtistType = ArtistType.Band };

        }
    }
}
