using System;

namespace STCP_API.Entities
{
    public class IncomingBus
    {
        public int LineNumber { get; set; }

        public string LineName { get; set; }

        public DateTime EstimatedTime { get; set; }

        public int WaitingTime { get; set; }
    }
}