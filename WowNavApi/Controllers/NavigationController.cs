using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WowNavApi.Services;
using WowNavBase;

namespace WowNavApi.Controllers
{
    [ApiController]
    public class NavigationController : ControllerBase
    {
        private readonly ILogger<NavigationController> logger;
        private readonly INavigationService navigationService;

        public NavigationController(ILogger<NavigationController> logger, INavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
        }

        [HttpPost]
        [Route("Navigation/CalculatePath")]
        public IActionResult CalculatePath(CalculatePathParameters parameters)
        {
            if (parameters.Start == null)
                return BadRequest($"{nameof(parameters.Start)} cannot be null.");
            if (parameters.End == null)
                return BadRequest($"{nameof(parameters.End)} cannot be null.");

            try
            {
                var path = navigationService.CalculatePath(parameters.MapId, parameters.Start, parameters.End, parameters.StraightPath);

                logger.LogInformation(
                    "Path requested for MapId={mapId} Start={start} End={end} StraightPath={straightPath}. ResultLength={resultLength}",
                    parameters.MapId,
                    parameters.Start,
                    parameters.End,
                    parameters.StraightPath,
                    path.Length);

                return Ok(path);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while calculating path.");

                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }
    }

    public class CalculatePathParameters
    {
        public uint MapId { get; set; }

        public Position Start { get; set; }

        public Position End { get; set; }

        public bool StraightPath { get; set; }
    }
}
