using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class ExceptionDefinition
    {
        public IEnumerable<string> RegexTemplates { get; set; }

        public Position Position { get; set; }
    }
}
