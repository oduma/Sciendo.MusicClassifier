using Sciendo.ArtistClassifier.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistClassifier.Contracts
{
    public interface IArtistProcessor
    { 
        IEnumerable<Artist> GetArtists(string proposedArtist, bool isComposer, bool isFeaturedArtist);

    }
}
