using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Models
{
    public class ArtistSummary
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ArtistType { get; set; }

        public bool ValidatedManually { get; set; }
    }
}
