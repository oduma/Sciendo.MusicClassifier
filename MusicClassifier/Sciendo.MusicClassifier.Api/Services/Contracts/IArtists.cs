using Sciendo.MusicClassifier.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services.Contracts
{
    public interface IArtists
    {
        ArtistsSummary GetArtists(string artistFileName, int pageNo, int pageSize);
    }
}
