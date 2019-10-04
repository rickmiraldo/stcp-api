using System.Collections.Generic;

namespace STCP_API.Entities
{
    public class Stop
    {
        public string Name { get; set; }

        public List<IncomingBus> IncomingBuses { get; set; }
    }
}
