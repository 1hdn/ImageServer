namespace ImageServer
{
    public class RequestHandlerResult : IRequestHandlerResult
    {
        public ResultStatus Status { get; }
        public IImageInfo ImageInfo { get; }

        public RequestHandlerResult(ResultStatus status) : this(status, null)
        {
        }

        public RequestHandlerResult(ResultStatus status, IImageInfo imageInfo)
        {
            Status = status;
            ImageInfo = imageInfo;
        }
    }
}
