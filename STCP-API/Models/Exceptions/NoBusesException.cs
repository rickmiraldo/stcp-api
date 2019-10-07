using System;

namespace STCP_API.Models.Exceptions
{
    public class NoBusesException : Exception
    {
        public NoBusesException(string message)
            : base("No buses programmed for stop: " + message)
        {
        }
    }
}
