using Microsoft.AspNetCore.Mvc;
using STCP_API.Models.Clients;
using System;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineController : Controller
    {
        // GET line/{lineNumber}/{direction?}/{getIncoming?}
        [HttpGet("{lineNumber}/{direction?}/{getIncoming?}")]
        public async Task<IActionResult> GetStopsFromLine(string lineNumber, string direction = "0", string getIncoming = "")
        {
            try
            {
                var result = await LineClient.GetStopsFromLine(lineNumber, direction, getIncoming);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}