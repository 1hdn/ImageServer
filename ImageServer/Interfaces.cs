using System;
using System.Threading.Tasks;

namespace ImageServer
{
    public interface IRequestHandler
    {
        /// <summary>
        /// Resizes and optionally optimizes images based on the instructions given in a formatted URL.
        /// </summary>
        /// <param name="url">A special formatted URL. See documentation for further details.</param>
        /// <param name="ipAddress">Remote users ip-address to be used for rate-limiting.</param>
        /// <returns></returns>
        Task<IRequestHandlerResult> HandleRequest(Uri url, string ipAddress);
    }

    public interface IRequestHandlerResult
    {
        /// <summary>
        /// An HTTP status code.
        /// </summary>
        ResultStatus Status { get; }

        /// <summary>
        /// Image size, type and location information.
        /// </summary>
        IImageInfo ImageInfo { get; }
    }

    public interface IImageInfo
    {
        /// <summary>
        /// The size of an image in bytes.
        /// </summary>
        long ByteLength { get; }
        
        /// <summary>
        /// The MIME type of an image.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// The absolute path to an image.
        /// </summary>
        string FilePath { get; }
    }

    public interface ILocation
    {
        /// <summary>
        /// The fully qualified URL to an image. 
        /// </summary>
        Uri AbsoluteUrl { get; }

        /// <summary>
        /// The absolute path to an image on the file system.
        /// </summary>
        string AbsolutePath { get; }
    }

    public interface ISettings
    {
        /// <summary>
        /// The location of the directory that contains the source/master images. This can be either an absolute path to a directory on the file system, or a fully qualified URL. 
        /// </summary>
        string SourceRoot { get; }

        /// <summary>
        /// The file system and URL locations of the generated image.
        /// </summary>
        ILocation DestinationRoot { get; }

        /// <summary>
        /// Configuration of rate-limiting.
        /// </summary>
        IRateLimiterSettings RateLimiterSettings { get; }

        /// <summary>
        /// Configuration of logging.
        /// </summary>
        ILoggerSettings LoggerSettings { get; }

        /// <summary>
        /// Configuration of gif-images.
        /// </summary>
        IGifSettings GifSettings { get; }

        /// <summary>
        /// Configuration of jpeg-images.
        /// </summary>
        IJpegSettings JpegSettings { get; }

        /// <summary>
        /// Configuration of png-images.
        /// </summary>
        IPngSettings PngSettings { get; }
    }

    public interface IRateLimiterSettings
    {
        /// <summary>
        /// Enables or disables rate-limiting. 
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// A time period in which the maximum requests and bytes apply for a single ip address.
        /// </summary>
        int TimeWindowInMinutes { get; }

        /// <summary>
        /// The maximum number of request to be served in the time window for a single ip address.
        /// </summary>
        int MaxRequestsInTimeWindow { get; }

        /// <summary>
        /// The maximum number of source image bytes to be processed in a given time window for a single ip address.
        /// </summary>
        long MaxBytesInTimeWindow { get; }

        /// <summary>
        /// A list of ip addresses that should not be affected by rate-limiting.
        /// </summary>
        string[] Whitelist { get; }

        /// <summary>
        /// A list of ip addresses that should not be served.
        /// </summary>
        string[] Blacklist { get; }
    }

    public interface ILoggerSettings
    {
        /// <summary>
        /// A flag indicating if information-type messages should be written to the log.
        /// </summary>
        bool WriteInfo { get; }

        /// <summary>
        /// A flag indicating if warning-type messages should be written to the log.
        /// </summary>
        bool WriteWarnings { get; }

        /// <summary>
        /// A flag indicating if error-type messages should be written to the log.
        /// </summary>
        bool WriteErrors { get; }

        /// <summary>
        /// A flag indicating if the log should write messages to the console.
        /// </summary>
        bool ToConsole { get; }

        /// <summary>
        /// A flag indicating if the log should write messages to a file.
        /// </summary>
        bool ToFile { get; }

        /// <summary>
        /// The absolute path to a directory on the file system in which to write log files.
        /// </summary>
        string Directory { get; }
    }

    public interface IPostProcessorSettings
    {
        /// <summary>
        /// If enabled, a post processing command will be executed on the generated image.
        /// </summary>
        bool PostProcessorEnabled { get; }

        /// <summary>
        /// A command line command to be executed on the generated image.
        /// </summary>
        string PostProcessorCommand { get; }
    }

    public interface IGifSettings : IPostProcessorSettings
    {
    }

    public interface IJpegSettings : IPostProcessorSettings
    {
        /// <summary>
        /// A quality setting in the range 0-100.
        /// </summary>
        int Quality { get; }
    }

    public interface IPngSettings : IPostProcessorSettings
    {
        /// <summary>
        /// A compression level in the range 0-9.
        /// </summary>
        int CompressionLevel { get; }
    }

    public interface ILogger
    {
        /// <summary>
        /// Writes an information-type message to the log.
        /// </summary>
        void LogInfo(string message);

        /// <summary>
        /// Writes a warning-type message to the log.
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Writes an error-type message to the log.
        /// </summary>
        void LogError(string message);
    }
}
