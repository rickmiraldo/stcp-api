using Microsoft.AspNetCore.Mvc;
using STCP_API.Models.Clients;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineController : Controller
    {
        // GET line/{lineNumber}
        [HttpGet("{lineNumber}")]
        public async Task<IActionResult> GetStopsFromLine(string lineNumber)
        {
            var result = await LineClient.GetStopsFromLine(lineNumber);
            return Ok(result);
        }

        // GET /line/full/{lineNumber}
        [HttpGet("full/{lineNumber}")]
        public async Task<IActionResult> GetStopsAndBusesFromLine(string lineNumber)
        {
            var result = await LineClient.GetStopsFromLine(lineNumber, true);
            return Ok(result);
        }
    }
}