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
        public IActionResult GetStopsFromLine(string lineNumber)
        {
            var result = LineClient.GetStopsFromLine(lineNumber);
            return Ok(result);
        }
    }
}