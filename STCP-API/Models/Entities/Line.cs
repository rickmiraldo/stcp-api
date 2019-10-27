using System.Collections.Generic;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class Line
    {
        public string Number { get; private set; }
        public string LineDirection { get; set; }
        public List<Stop> Stops { get; set; }

        public Line(string number, string lineDirection, List<Stop> stops)
        {
            Number = number;
            LineDirection = lineDirection;
            Stops = stops;
        }
    }
}
