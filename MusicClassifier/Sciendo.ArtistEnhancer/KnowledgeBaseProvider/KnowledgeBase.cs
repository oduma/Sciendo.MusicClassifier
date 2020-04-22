using Sciendo.ArtistEnhancer.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistEnhancer.KnowledgeBaseProvider
{
    public class KnowledgeBase
    {
        public Dictionary<string,string[]> MembersMarkersByLanguages { get; set; }

        public Dictionary<string, string> Noise { get; set; }

        public Dictionary<string, string> LatinAlphabetTransformations { get; set; }
    }
}
