using Sciendo.ArtistClassifier.NLP.NER.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Contracts
{
    public interface IArtistClassifier
    {
        Artist Classify(string input);
    }
}
