﻿using CsvHelper.Configuration;
using Sciendo.MusicClassifier.Api.Services.IoAccess.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services.IoAccess.Mappers
{
    public class ArtistNodeMap : ClassMap<ArtistNode>
    {
        public ArtistNodeMap()
        {
            Map(m => m.ArtistId).Name("artistID:ID(Artist)");
            Map(m => m.Name).Name("name");
            Map(m => m.WikiPage).Name("wikiPage");
            Map(m => m.ArtistInCollection).Name("artistInCollection");
            Map(m => m.ManualValidated).Name("manualValidated");
            Map(m => m.ArtistLabel).Name(":LABEL");
        }
    }
}
