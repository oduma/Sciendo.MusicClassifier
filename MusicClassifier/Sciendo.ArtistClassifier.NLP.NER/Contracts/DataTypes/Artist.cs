using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Contracts.DataTypes
{
    public class Artist
    {
        public string Name { get; set; }

        public ArtistType ArtistType { get; set; }
    }
}
