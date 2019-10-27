using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCP_API.Models.Entities
{
    public class LocatedBus
    {
        public string Destination { get; set; }
        public string NextStopId { get; set; }
        public string NextStopName { get; set; }
        public string NextZone { get; set; }
        public DateTime Eta { get; set; }
        public int WaitingTime { get; set; }

        public LocatedBus(string destination, string nextStopId, string nextStopName, string nextZone, DateTime eta, int waitingTime)
        {
            Destination = destination;
            NextStopId = nextStopId;
            NextStopName = nextStopName;
            NextZone = nextZone;
            Eta = eta;
            WaitingTime = waitingTime;
        }
    }
}
