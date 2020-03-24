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
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file))
                throw new ArgumentException($"{file} does not exist.");
            return JsonConvert.DeserializeObject<KnowledgeBase>(File.ReadAllText(file));
        }
    }
}
