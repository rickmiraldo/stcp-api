using Microsoft.AspNetCore.Mvc;
using STCP_API.Models;
using System;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StopController : Controller
    {
        // GET stop/{busStop}
        [HttpGet("{busStop}")]
        public async Task<IActionResult> GetBusesFromStop(string busStop)
        {
            try
            {
                var result = await StopClient.GetNextBuses(busStop);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}