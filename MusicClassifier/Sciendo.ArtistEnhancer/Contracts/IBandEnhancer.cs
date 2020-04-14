using Sciendo.ArtistEnhancer.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistEnhancer.Contracts
{
    public interface IBandEnhancer
    {
        BandWikiPageInfo FindBandInWikipedia(string bandName);

    }
}
