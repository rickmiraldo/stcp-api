using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCP_API.Models.Entities
{
    public class LocatedLine
    {
        public string Number { get; set; }
        public string Direction { get; set; }
        public List<LocatedBus> LocatedBuses { get; set; }

        public LocatedLine(string number, string direction)
        {
            Number = number;
            Direction = direction;
            LocatedBuses = new List<LocatedBus>();
        }
    }
}
