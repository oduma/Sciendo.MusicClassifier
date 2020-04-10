using edu.stanford.nlp.ie.crf;
using java.util;
using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using Sciendo.ArtistClassifier.NLP.NER.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Logic
{
    public class ArtistClassifier : IArtistClassifier
    {
        private readonly NlpConfig nlpConfig;

        public ArtistClassifier(NlpConfig nlpConfig)
        {
            this.nlpConfig = nlpConfig;
        }
        public Artist Classify(string input)
        {
            var classifier = GetNerClassifier();
            var crfWithXml = ParseToCrf(classifier, input);
            var personNames = IsolatePersonNames(crfWithXml);
            if(personNames.Count()!=1)
            {
                return new Artist { Name = input, ArtistType = ArtistType.Band };
            }
            if(personNames.First()==input)
            {
                return new Artist { Name = input, ArtistType = ArtistType.Artist };
            }
            return new Artist { Name = input, ArtistType = ArtistType.Band };

        }

        private IEnumerable<string> IsolatePersonNames(string crfWithXml)
        {
            var results = Regex.Matches(crfWithXml, nlpConfig.PersonsIsolatorRegEx);
            foreach(var result in results)
            {
                if (!string.IsNullOrEmpty(result.ToString().Trim()))
                    yield return result.ToString().Trim();
            }
        }

        private string ParseToCrf(CRFClassifier classifier, string input)
        {
            return classifier.classifyWithInlineXML(input);
        }

        private CRFClassifier GetNerClassifier()
        {
            return CRFClassifier.getClassifierNoExceptions(nlpConfig.JarRoot + nlpConfig.ClassifiersDirectory + nlpConfig.Ner3ClassesModel);
        }
    }
}
