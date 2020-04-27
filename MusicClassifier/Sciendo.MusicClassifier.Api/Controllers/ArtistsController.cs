using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sciendo.MusicClassifier.Api.Models;
using Sciendo.MusicClassifier.Api.Services.Contracts;

namespace Sciendo.MusicClassifier.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly ILogger<ArtistsController> logger;
        private readonly ISets setsService;
        private readonly IArtists artistsService;

        public ArtistsController(ILogger<ArtistsController> logger, ISets setsService, IArtists artistsService)
        {
            this.logger = logger;
            this.setsService = setsService;
            this.artistsService = artistsService;
        }
        [HttpGet]
        public ActionResult<ArtistsSummary> Get(string setName, int pageNo, int pageSize)
        {
            try
            {
                var filesInSet = setsService.GetAllFilesInSet(setName);

                return Ok(artistsService.GetArtists(filesInSet[Configuration.SetFileType.Artist],pageNo,pageSize));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Service Exception");
                return StatusCode(500, "Service Exception.");
            }
        }
    }
}