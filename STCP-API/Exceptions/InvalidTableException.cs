using System;

namespace STCP_API.Exceptions
{
    public class InvalidTableException : Exception
    {
        public InvalidTableException(string message)
            : base("Invalid table response for stop: " + message)
        {
        }
    }
}
