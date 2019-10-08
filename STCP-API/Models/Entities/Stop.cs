﻿using System.Collections.Generic;
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BusStopId);
            sb.Append(" - ");
            sb.Append(BusStopName);

            if (!string.IsNullOrEmpty(Zone))
            {
                sb.Append(" (");
                sb.Append(Zone);
                sb.Append(")");
            }

            sb.Append("\r\n==========\r\n");

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
