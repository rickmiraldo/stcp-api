using Microsoft.AspNetCore.Mvc;
using STCP_API.Clients;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StopController : Controller
    {
        // GET stop/get/{id}
        [HttpGet("get/{id}")]
        public async Task<string> Get(string id)
        {
            var result = await StopClient.GetNextBuses(id);

            return result.ToString();
        }
    }
}