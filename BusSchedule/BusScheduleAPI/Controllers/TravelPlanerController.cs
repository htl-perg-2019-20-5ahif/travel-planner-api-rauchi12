using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BusScheduleLibrary;

namespace BusScheduleAPI.Controllers
{
    [ApiController]
    [Route("/api/travelplan")]
    public class TravelPlanerController : ControllerBase
    {

        private readonly ILogger<TravelPlanerController> _logger;
        private static HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("https://cddataexchange.blob.core.windows.net") };

        public TravelPlanerController(ILogger<TravelPlanerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string from, [FromQuery] string to, [FromQuery] string start)
        {
            var jsonFile = await HttpClient.GetAsync("/data-exchange/htl-homework/travelPlan.json");
            jsonFile.EnsureSuccessStatusCode();
            var responseBody = await jsonFile.Content.ReadAsStringAsync();
            var routes = JsonSerializer.Deserialize<Route[]>(responseBody);

            var finder = new ConnectionFinder(routes);
            var trip = finder.FindConnection(from, to, start);
            if (trip == null)
            {
                Console.WriteLine("Sorry, no connection.");
                return NoContent();
            }
            return Ok(trip);
        }
    }
}
