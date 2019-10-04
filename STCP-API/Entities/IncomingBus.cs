using System;
using System.Text;

namespace STCP_API.Entities
{
    public class IncomingBus
    {
        public string LineNumber { get; set; }

        public string LineName { get; set; }

        public DateTime EstimatedTime { get; set; }

        public int WaitingTime { get; set; }

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