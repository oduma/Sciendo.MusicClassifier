using Sciendo.ArtistClassifier.Logic;
using Sciendo.MusicClassifier.KnowledgeBaseProvider;
using System;
using System.IO;

namespace AC
{
    class Program
    {
        static void Main(string[] args)
        {
            var artistProcessor = new ArtistProcessor(new KnowledgeBaseFactory(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KnowledgebaseProvider\\newknowledgebase.json"));
            Console.WriteLine("Artist Name:");
            var proposedName = Console.ReadLine();
            while(proposedName!=null)
            {
                var result = artistProcessor.GetArtists(proposedName,true,false);
                if(result!=null)
                    foreach(var artist in result)
                    {
                        Console.WriteLine("Found: {1} - {0}", artist.Name, artist.Type);
                    }
                proposedName = Console.ReadLine();
            }

        }
    }
}
