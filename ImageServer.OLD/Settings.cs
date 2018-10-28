using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ImageServer.Tests")]
namespace ImageServer
{
    internal class Settings : ISettings
    {
        public string SourceRoot { get; }

        public string DestinationRoot { get; }

        public IRateLimiterSettings RateLimiterSettings { get; }

        public ILoggerSettings LoggerSettings { get; }

        public IGifSettings GifSettings { get; }

        public IJpegSettings JpegSettings { get; }

        public IPngSettings PngSettings { get; }

        public Settings(IConfiguration configuration)
        {
            SourceRoot = configuration.GetValue("SourceRoot", string.Empty);
            DestinationRoot = configuration.GetValue("DestinationRoot", string.Empty);

            if (string.IsNullOrEmpty(SourceRoot))
            {
                throw new SettingsException("SourceRoot must be set");
            }

            if (string.IsNullOrEmpty(DestinationRoot))
            {
                throw new SettingsException("DestinationRoot must be set");
            }

            RateLimiterSettings = new RateLimiterSettings(configuration.GetSection("RateLimiterSettings"));
            LoggerSettings = new LoggerSettings(configuration.GetSection("LoggerSettings"));
            GifSettings = new GifSettings(configuration.GetSection("GifSettings"));
            JpegSettings = new JpegSettings(configuration.GetSection("JpegSettings"));
            PngSettings = new PngSettings(configuration.GetSection("PngSettings"));
        }
    }

    internal class RateLimiterSettings : IRateLimiterSettings
    {
        public bool Enabled { get; }

        public int TimeWindowInMinutes { get; }

        public int MaxRequestsInTimeWindow { get; }

        public long MaxBytesInTimeWindow { get; }

        public string[] Whitelist { get; }

        public string[] Blacklist { get; }

        public RateLimiterSettings(IConfiguration configuration)
        {
            Enabled = configuration.GetValue("Enabled", false);
            TimeWindowInMinutes = configuration.GetValue("TimeWindowInMinutes", 1);
            MaxRequestsInTimeWindow = configuration.GetValue("MaxRequestsInTimeWindow", 60);
            MaxBytesInTimeWindow = configuration.GetValue("MaxBytesInTimeWindow", 1024 * 1024 * 5);
            Whitelist = GetArray(configuration.GetSection("Whitelist"));
            Blacklist = GetArray(configuration.GetSection("Blacklist"));
        }

        private string[] GetArray(IConfigurationSection section)
        {
            return section.GetChildren().ToList().Select(x => x.Value).ToArray();
        }
    }

    internal class LoggerSettings : ILoggerSettings
    {
        public bool WriteInfo { get; }

        public bool WriteWarnings { get; }

        public bool WriteErrors { get; }

        public bool ToConsole { get; }

        public bool ToFile { get; }

        public string Directory { get; }

        public LoggerSettings(IConfiguration configuration)
        {
            WriteInfo = configuration.GetValue("WriteInfo", false);
            WriteWarnings = configuration.GetValue("WriteWarnings", false);
            WriteErrors = configuration.GetValue("WriteErrors", false);
            ToConsole = configuration.GetValue("ToConsole", false);
            ToFile = configuration.GetValue("ToFile", false);
            Directory = configuration.GetValue("Directory", string.Empty);
        }
    }

    internal abstract class PostProcessorSettings : IPostProcessorSettings
    {
        public string PostProcessorPath { get; }

        public string PostProcessorArgs { get; }

        public PostProcessorSettings(IConfiguration configuration)
        {
            PostProcessorPath = configuration.GetValue("PostProcessorPath", string.Empty);
            PostProcessorArgs = configuration.GetValue("PostProcessorArgs", string.Empty);
        }
    }

    internal class GifSettings : PostProcessorSettings, IGifSettings
    {
        public GifSettings(IConfiguration configuration) : base(configuration)
        {
        }
    }

    internal class JpegSettings : PostProcessorSettings, IJpegSettings
    {
        public int Quality { get; }

        public JpegSettings(IConfiguration configuration) : base(configuration)
        {
            Quality = configuration.GetValue("Quality", 75);
        }
    }

    internal class PngSettings : PostProcessorSettings, IPngSettings
    {
        public int CompressionLevel { get; }

        public PngSettings(IConfiguration configuration) : base(configuration)
        {
            CompressionLevel = configuration.GetValue("CompressionLevel", 6);
        }
    }

    internal class SettingsException : Exception
    {
        public SettingsException(string message) : base(message)
        {
        }
    }
}
