using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    //x
    public class ExceptionDefinition
    {
        public Position Position { get; set; }

        public IEnumerable<string> RegexTemplates { get; set; }
    }
}
