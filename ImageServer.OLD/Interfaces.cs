using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageServer
{
    public interface ISettings
    {
        string SourceRoot { get; }
        string DestinationRoot { get; }
        IRateLimiterSettings RateLimiterSettings { get; }
        ILoggerSettings LoggerSettings { get; }
        IGifSettings GifSettings { get; }
        IJpegSettings JpegSettings { get; }
        IPngSettings PngSettings { get; }
    }

    public interface IRateLimiterSettings
    {
        bool Enabled { get; }
        int TimeWindowInMinutes { get; }
        int MaxRequestsInTimeWindow { get; }
        long MaxBytesInTimeWindow { get; }
        string[] Whitelist { get; }
        string[] Blacklist { get; }
    }

    public interface ILoggerSettings
    {
        bool WriteInfo { get; }
        bool WriteWarnings { get; }
        bool WriteErrors { get; }
        bool ToConsole { get; }
        bool ToFile { get; }
        string Directory { get; }
    }

    public interface IPostProcessorSettings
    {
        string PostProcessorPath { get; }
        string PostProcessorArgs { get; }
    }

    public interface IGifSettings : IPostProcessorSettings
    {
    }

    public interface IJpegSettings : IPostProcessorSettings
    {
        int Quality { get; }
    }

    public interface IPngSettings : IPostProcessorSettings
    {
        int CompressionLevel { get; }
    }




    public interface IRateLimiter
    {
        bool IsBlacklisted(string ipAddress);
        bool IsLimitExceeded(string ipAddress);
        void RegisterRequest(string ipAddress, long bytes);
    }

    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
