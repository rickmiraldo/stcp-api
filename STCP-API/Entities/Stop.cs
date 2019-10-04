using System.Collections.Generic;
using System.Text;

namespace STCP_API.Entities
{
    public class Stop
    {
        public string Name { get; set; }

        public List<IncomingBus> IncomingBuses { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append("\r\n=====\r\n");
            foreach (var bus in IncomingBuses)
            {
                sb.Append(bus.ToString());
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    }
}
