using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sciendo.MusicClassifier.Api.Services.Contracts;

namespace Sciendo.MusicClassifier.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetsController : ControllerBase
    {
        private readonly ILogger<SetsController> logger;
        private readonly ISets sets;

        public SetsController(ILogger<SetsController> logger, ISets sets)
        {
            this.logger = logger;
            this.sets = sets;
        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                return Ok(sets.GetAllSets());
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Service Exception");
                return StatusCode(500, "Service Exception.");
            }
        }


    }
}