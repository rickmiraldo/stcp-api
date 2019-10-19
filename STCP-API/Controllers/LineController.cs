using Microsoft.AspNetCore.Mvc;
using STCP_API.Models.Clients;
using STCP_API.Models.Entities;
using System;
using System.Collections.Generic;
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

        // GET line/all/{getStops?}
        [HttpGet("all/{getStops?}")]
        public async Task<IActionResult> GetAllLines(string getStops = "")
        {
            try
            {
                var result = await LineClient.GetAllLines(getStops);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}