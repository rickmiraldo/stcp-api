using System.Collections.Generic;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class Stop
    {
        public string BusStopId { get; private set; }
        public string BusStopName { get; private set; }
        public string Zone { get; set; }
        public List<IncomingBus> IncomingBuses { get; set; }

        public Stop(string busStopId, List<IncomingBus> incomingBuses, string busStopName = "", string zone = "")
        {
            BusStopId = busStopId;
            BusStopName = busStopName;
            Zone = zone;
            IncomingBuses = incomingBuses;
        }
    }
}
