namespace ImageServer
{
    public enum ResultStatus
    {
        OK = 200,
        BadRequest = 400,
        Forbidden = 403,
        FileNotFound = 404,
        TooManyRequests = 429,
        ServerError = 500,
        BadGateway = 502
    }
}
