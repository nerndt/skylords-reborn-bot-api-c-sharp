using System;

namespace SkylordsRebornAPI.Exceptions
{
    public class BackendUnknownException : Exception
    {
        public BackendUnknownException()
        {
        }

        public BackendUnknownException(string message) : base(message)
        {
        }

        public BackendUnknownException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}