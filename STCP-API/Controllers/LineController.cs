using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineController : Controller
    {
        // GET line/{lineNumber}
        [HttpGet("{lineNumber}")]
        public async Task<IActionResult> GetAllStopsFromLine(string lineNumber)
        {
            return View();
        }
    }
}