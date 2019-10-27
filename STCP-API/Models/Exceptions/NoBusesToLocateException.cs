using System;

namespace STCP_API.Models.Exceptions
{
    public class NoBusesToLocateException : Exception
    {
        public NoBusesToLocateException(string message)
            : base("No buses to locate in line: " + message)
        {
        }
    }
}
