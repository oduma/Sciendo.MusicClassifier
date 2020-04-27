using Microsoft.Extensions.Logging;
using Sciendo.MusicClassifier.Api.Models;
using Sciendo.MusicClassifier.Api.Services.Contracts;
using Sciendo.MusicClassifier.Api.Services.IoAccess;
using Sciendo.MusicClassifier.Api.Services.IoAccess.DataTypes;
using Sciendo.MusicClassifier.Api.Services.IoAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services
{
    public class Artists : IArtists
    {
        private readonly ILogger<Artists> logger;

        public Artists(ILogger<Artists> logger)
        {
            this.logger = logger;
        }
        public ArtistsSummary GetArtists(string artistFileName, int pageNo, int pageSize)
        {
            if (string.IsNullOrEmpty(artistFileName))
                throw new ArgumentNullException(nameof(artistFileName));
            if (pageSize == 0)
                throw new ArgumentException("Pagesize 0 not supported.");
            var artists = IoManager.ReadWithMapper<ArtistNode, ArtistNodeMap>(artistFileName);
            var artistsSummary = new ArtistsSummary 
            { 
                PageInfo = new PageInfo 
                { 
                    PageNumber = pageNo, 
                    PageSize = pageSize, 
                    TotalNumberOfPages = artists.Count / pageSize 
                }, 
                ArtistsSummaries= artists.Where(a=>a.ArtistInCollection)
                    .Skip(pageNo * pageSize)
                    .Take(pageSize)
                    .Select(a => new ArtistSummary 
                    { 
                        Id = a.ArtistId, 
                        ArtistType = a.ArtistLabel, 
                        Name = a.Name, 
                        ValidatedManually = a.ManualValidated 
                    }).ToArray()
            };
            return artistsSummary;
        }
    }
}
