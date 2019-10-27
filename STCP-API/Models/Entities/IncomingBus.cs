using System;
using System.Text;

namespace STCP_API.Models.Entities
{
    public class IncomingBus
    {
        public string LineNumber { get; private set; }
        public string Destination { get; private set; }
        public DateTime EstimatedTime { get; set; }
        public int WaitingTime { get; set; }

        public IncomingBus(string lineNumber, string destination, DateTime estimatedTime, int waitingTime)
        {
            LineNumber = lineNumber;
            Destination = destination;
            EstimatedTime = estimatedTime;
            WaitingTime = waitingTime;
        }
    }
}