using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services.IoAccess.DataTypes
{
    public class ArtistNode
    {
        [Name("artistID:ID(Artist)")]
        public Guid ArtistId { get; set; }
        [Name("name")]
        public string Name { get; set; }
        [Name(":LABEL")]
        public string ArtistLabel { get; set; }

        [Name("wikiPage")]

        public string WikiPage { get; set; }

        [Name("artistInCollection")]
        public bool ArtistInCollection { get; set; }

        [Name("manualValidated")]
        public bool ManualValidated { get; set; }

    }
}
