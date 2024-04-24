using System;

namespace SkylordsRebornAPI.Exceptions
{
    public class BackendCachingException : Exception
    {
        public BackendCachingException()
        {
        }

        public BackendCachingException(string message) : base(message)
        {
        }

        public BackendCachingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}