using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class KnowledgeBaseFactory : IKnowledgeBaseFactory
    {
        public KnowledgeBase GetKnowledgeBase(string file)
        {
            return JsonConvert.DeserializeObject<KnowledgeBase>(File.ReadAllText(file));
        }
    }
}
