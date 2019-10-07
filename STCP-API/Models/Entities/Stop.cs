using System.Collections.Generic;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class Stop
    {
        public string BusStopName { get; private set; }
        public List<IncomingBus>? IncomingBuses { get; set; }

        public Stop(string busStopName, List<IncomingBus> incomingBuses)
        {
            BusStopName = busStopName;
            IncomingBuses = incomingBuses;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BusStopName);
            sb.Append("\r\n=====\r\n");

            if (IncomingBuses == null)
            {
                sb.Append("Incoming buses unavailable!");
            }
            else
            {
                foreach (var bus in IncomingBuses)
                {
                    sb.Append(bus.ToString());
                    sb.Append("\r\n");
                }
            }

            return sb.ToString();
        }
    }
}
