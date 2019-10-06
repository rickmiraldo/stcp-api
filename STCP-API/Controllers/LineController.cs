using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineController : Controller
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllStopsFromLine(string lineNumber)
        {
            return View();
        }
    }
}