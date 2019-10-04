using System;

namespace STCP_API.Exceptions
{
    public class NoBusesException : Exception
    {
        public NoBusesException(string message)
            : base("No buses programmed for stop: " + message)
        {
        }
    }
}
