using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistEnhancer.Contracts.DataTypes
{
    public class BandWikiPageInfo
    {
        public long PageId { get; set; }

        public string Language { get; set; }

        public string Name { get; set; }

        public List<string> Members { get; set; }
    }
}
