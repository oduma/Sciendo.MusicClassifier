using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Configuration
{
    public class NlpConfig
    {
        public string JarRoot { get; set; }

        public string ClassifiersDirectory { get; set; }

        public string Ner3ClassesModel { get; set; }

        //(?<=\<PERSON\>).*?(?=\<\/PERSON\>)
        public string PersonsIsolatorRegEx { get; set; }
    }
}
