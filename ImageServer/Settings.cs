using System;

namespace ImageServer
{
    public class Settings : ISettings
    {
        public string SourceRoot { get; }

        public ILocation DestinationRoot { get; }

        public IRateLimiterSettings RateLimiterSettings { get; }

        public ILoggerSettings LoggerSettings { get; }

        public IGifSettings GifSettings { get; }

        public IJpegSettings JpegSettings { get; }

        public IPngSettings PngSettings { get; }

        public Settings(
            string sourceRoot, 
            ILocation destinationRoot, 
            IRateLimiterSettings rateLimiterSettings = null, 
            ILoggerSettings loggerSettings = null, 
            IGifSettings gifSettings = null, 
            IJpegSettings jpegSettings = null, 
            IPngSettings pngSettings = null)
        {
            if (string.IsNullOrEmpty(sourceRoot))
            {
                throw new ArgumentException("SourceRoot must be set.", "sourceRoot");
            }

            if (destinationRoot == null)
            {
                throw new ArgumentException("DestinationRoot must be set.", "destinationRoot");
            }

            SourceRoot = sourceRoot;
            DestinationRoot = destinationRoot;
            RateLimiterSettings = rateLimiterSettings ?? new RateLimiterSettings();
            LoggerSettings = loggerSettings ?? new LoggerSettings();
            GifSettings = gifSettings ?? new GifSettings();
            JpegSettings = jpegSettings ?? new JpegSettings();
            PngSettings = pngSettings ?? new PngSettings();
        }
    }

    public class RateLimiterSettings : IRateLimiterSettings
    {
        public bool Enabled { get; set; }

        public int TimeWindowInMinutes { get; set; } = 10;

        public int MaxRequestsInTimeWindow { get; set; } = 1000;

        public long MaxBytesInTimeWindow { get; set; } = 1024 * 1024 * 100;

        public string[] Whitelist { get; set; } = new string[0];

        public string[] Blacklist { get; set; } = new string[0];
    }

    public class LoggerSettings : ILoggerSettings
    {
        public bool WriteInfo { get; set; }

        public bool WriteWarnings { get; set; }

        public bool WriteErrors { get; set; }

        public bool ToConsole { get; set; }

        public bool ToFile { get; set; }

        public string Directory { get; set; } = string.Empty;
    }

    public abstract class PostProcessorSettings : IPostProcessorSettings
    {
        public bool PostProcessorEnabled { get; set; }

        public string PostProcessorCommand { get; set; } = string.Empty;
    }

    public class GifSettings : PostProcessorSettings, IGifSettings
    {
    }

    public class JpegSettings : PostProcessorSettings, IJpegSettings
    {
        public int Quality { get; set; } = 75;
    }

    public class PngSettings : PostProcessorSettings, IPngSettings
    {
        public int CompressionLevel { get; set; } = 6;
    }
}
