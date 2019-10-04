using System;

namespace STCP_API.Exceptions
{
    public class InvalidBusStopNameException : Exception
    {
        public InvalidBusStopNameException(string message)
            : base("Invalid bus stop name: " + message)
        {
        }
    }
}
