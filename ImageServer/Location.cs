using System;

namespace ImageServer
{
    public class Location : ILocation
    {
        public Uri AbsoluteUrl { get; }
        public string AbsolutePath { get; }

        public Location(Uri absoluteUrl, string absolutePath)
        {
            AbsoluteUrl = absoluteUrl;
            AbsolutePath = absolutePath;
        }
    }
}
