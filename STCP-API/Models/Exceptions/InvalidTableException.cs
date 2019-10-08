using System;

namespace STCP_API.Models.Exceptions
{
    public class InvalidTableException : Exception
    {
        public InvalidTableException(string message)
            : base("Invalid table for: " + message)
        {
        }
    }
}
