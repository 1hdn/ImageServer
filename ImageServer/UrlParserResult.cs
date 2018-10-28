namespace ImageServer
{
    internal enum ResizeMethod
    {
        Contain,
        Cover,
        Fill,
        ScaleDown
    }

    internal enum ImageType
    {
        Gif,
        Jpeg,
        Png
    }

    internal class UrlParserResult
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int? Quality { get; internal set; }
        public int? Compression { get; internal set; }
        public bool? PostProcess { get; internal set; }
        public ResizeMethod? ResizeMethod { get; internal set; }
        public string Source { get; internal set; }
        public string Destination { get; internal set; }
        public ImageType ImageType { get; internal set; }
    }
}
