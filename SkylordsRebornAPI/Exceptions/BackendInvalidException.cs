using System;

namespace SkylordsRebornAPI.Exceptions
{
    public class BackendInvalidException : Exception
    {
        public BackendInvalidException()
        {
        }

        public BackendInvalidException(string message) : base(message)
        {
        }

        public BackendInvalidException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}