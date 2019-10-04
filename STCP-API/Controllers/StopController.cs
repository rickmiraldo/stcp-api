using Microsoft.AspNetCore.Mvc;
using STCP_API.Clients;
using System.Threading.Tasks;

namespace STCP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StopController : Controller
    {
        public async Task<string> Index()
        {
            var result = await StopClient.GetNextBuses("LION3");

            return result;
        }
    }
}