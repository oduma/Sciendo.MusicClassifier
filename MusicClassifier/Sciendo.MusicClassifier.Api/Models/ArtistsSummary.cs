using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Models
{
    public class ArtistsSummary
    {
        public ArtistSummary[] ArtistsSummaries { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}
