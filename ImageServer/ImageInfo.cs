namespace ImageServer
{
    internal class ImageInfo : IImageInfo
    {
        public long ByteLength { get; }

        public string ContentType { get; }

        public string FilePath { get; }

        public ImageInfo(long byteLength, string contentType, string filePath)
        {
            ByteLength = byteLength;
            ContentType = contentType;
            FilePath = filePath;
        }
    }
}
