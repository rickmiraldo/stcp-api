using System;

namespace STCP_API.Models.Exceptions
{
    public class InvalidBusStopNameException : Exception
    {
        public InvalidBusStopNameException(string message)
            : base("Invalid bus stop name: " + message)
        {
        }
    }
}
