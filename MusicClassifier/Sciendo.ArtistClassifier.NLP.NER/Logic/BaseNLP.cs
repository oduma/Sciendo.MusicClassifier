using edu.stanford.nlp.ie.crf;
using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Logic
{
    public class BaseNLP
    {
        protected readonly NlpConfig nlpConfig;

        protected readonly CRFClassifier classifier;


        protected BaseNLP(NlpConfig nlpConfig)
        {
            this.nlpConfig = nlpConfig;
            this.classifier = GetNerClassifier();

        }

        protected string ParseToCrf(CRFClassifier classifier, string input)
        {
            return classifier.classifyWithInlineXML(input);
        }

        protected IEnumerable<string> IsolatePersonNames(string crfWithXml)
        {
            var results = Regex.Matches(crfWithXml, nlpConfig.PersonsIsolatorRegEx);
            foreach (var result in results)
            {
                if (!string.IsNullOrEmpty(result.ToString().Trim()))
                    yield return result.ToString().Trim();
            }
        }

        private CRFClassifier GetNerClassifier()
        {
            return CRFClassifier.getClassifierNoExceptions(nlpConfig.JarRoot + nlpConfig.ClassifiersDirectory + nlpConfig.Ner3ClassesModel);
        }


    }
}
