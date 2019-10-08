using System.Collections.Generic;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class Line
    {
        public string LineNumber { get; private set; }
        public string Direction { get; set; }
        public List<Stop> Stops { get; set; }

        public Line(string lineNumber, string direction, List<Stop> stops)
        {
            LineNumber = lineNumber;
            Direction = direction;
            Stops = stops;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(LineNumber);
            sb.Append(" - ");

            sb.Append("\r\n==========\r\n");

            if (Stops.Count == 0)
            {
                sb.Append("No stops available!");
            }
            else
            {
                foreach (var stop in Stops)
                {
                    sb.Append(stop.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
