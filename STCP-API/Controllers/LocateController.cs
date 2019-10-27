using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using STCP_API.Models;
using STCP_API.Models.Clients;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocateController : Controller
    {
        // GET locate/{lineNumber}/{direction?}
        // direction = "0" | "1"
        [HttpGet("{lineNumber}/{direction?}")]
        public async Task<IActionResult> LocateBuses(string lineNumber, string direction = "0")
        {
            try
            {
                var line = await LineClient.GetStopsFromLine(lineNumber, direction, "filter");
                var locatedBuses = Locate.LocateBusesFromLine(line);

                return Ok(locatedBuses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}