using System;
using System.IO;
using Newtonsoft.Json;
using Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators;

namespace KBG
{
    class Program
    {
        static void Main(string[] args)
        {
            var knowledgeBase = new KnowledgeBaseLoaded();
            File.WriteAllText(@"C:\Code\Sciendo\Sciendo.MusicClassifier\Sciendo.MusicClassifier\MusicClassifier\Sciendo.ArtistClassifier\KnowledgeBaseProvider\newknowledgebase.json", JsonConvert.SerializeObject(knowledgeBase));
        }
    }
}
