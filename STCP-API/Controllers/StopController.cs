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
        // GET stop/get/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusesFromStop(string id)
        {
            try
            {
                var result = await StopClient.GetNextBuses(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}