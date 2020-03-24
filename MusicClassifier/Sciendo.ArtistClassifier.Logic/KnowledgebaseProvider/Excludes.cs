using Sciendo.ArtistClassifier.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class Excludes
    {

        public string PlaceholderAlbumArtists { get; set; }

        public string[] WordsSeparatorsGlobal { get; set; }

        public string[] ArtistsForSplitting { get; set; }

        public string[] BandsForSplitting { get; set; }
    }
}
