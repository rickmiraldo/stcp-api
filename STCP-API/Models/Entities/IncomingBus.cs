using System;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class IncomingBus
    {
        public string LineNumber { get; private set; }
        public string LineName { get; private set; }
        public DateTime EstimatedTime { get; set; }
        public int WaitingTime { get; set; }

        public IncomingBus(string lineNumber, string lineName, DateTime estimatedTime, int waitingTime)
        {
            LineNumber = lineNumber;
            LineName = lineName;
            EstimatedTime = estimatedTime;
            WaitingTime = waitingTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LineNumber);
            sb.Append("-");
            sb.Append(LineName);
            sb.Append(" - ETA: ");
            sb.Append(EstimatedTime.ToString("HH:mm"));
            sb.Append(" (");
            if (WaitingTime == 0)
            {
                sb.Append("a passar");
            }
            else
            {
                sb.Append(WaitingTime);
                sb.Append(" min");
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}