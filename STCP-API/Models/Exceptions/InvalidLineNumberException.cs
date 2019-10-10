using System;

namespace STCP_API.Models.Exceptions
{
    public class InvalidLineNumberException : Exception
    {
        public InvalidLineNumberException(string message)
            : base("Invalid line number: " + message)
        {
        }
    }
}
