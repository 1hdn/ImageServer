using System;

namespace ImageServer
{
    internal class UrlParserException : Exception
    {
        public UrlParserException(string message) : base(message)
        {
        }
    }
}
