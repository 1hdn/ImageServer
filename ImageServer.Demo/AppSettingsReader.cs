using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ImageServer.Demo
{
    public class AppSettingsReader
    {
        internal static ISettings GetSettings(IConfiguration configuration)
        {
            string sourceRoot = configuration.GetValue<string>("SourceRoot");
            string destinationRootUrl = configuration.GetValue<string>("DestinationRootUrl");
            string destinationRootPath = configuration.GetValue<string>("DestinationRootPath");

            ILocation destinationRoot = new Location(new Uri(destinationRootUrl), destinationRootPath);

            IConfigurationSection rateLimiterConfig = configuration.GetSection("RateLimiter");
            IRateLimiterSettings rateLimiterSettings = new RateLimiterSettings
            {
                Enabled = rateLimiterConfig.GetValue("Enabled", false),
                TimeWindowInMinutes = rateLimiterConfig.GetValue("TimeWindowInMinutes", 10),
                MaxRequestsInTimeWindow = rateLimiterConfig.GetValue("MaxRequestsInTimeWindow", 100),
                MaxBytesInTimeWindow = rateLimiterConfig.GetValue("MaxBytesInTimeWindow", 1024 * 1024 * 50),
                Whitelist = rateLimiterConfig.GetSection("Whitelist").GetChildren().ToList().Select(x => x.Value).ToArray(),
                Blacklist = rateLimiterConfig.GetSection("Blacklist").GetChildren().ToList().Select(x => x.Value).ToArray()
            };

            IConfigurationSection loggerConfig = configuration.GetSection("Logger");
            ILoggerSettings loggerSettings = new LoggerSettings
            {
                WriteInfo = loggerConfig.GetValue("WriteInfo", false),
                WriteWarnings = loggerConfig.GetValue("WriteWarnings", false),
                WriteErrors = loggerConfig.GetValue("WriteErrors", false),
                ToConsole = loggerConfig.GetValue("ToConsole", false),
                ToFile = loggerConfig.GetValue("ToFile", false),
                Directory = loggerConfig.GetValue("Directory", string.Empty)
            };

            IConfigurationSection gifConfig = configuration.GetSection("GifSettings");
            IGifSettings gifSettings = new GifSettings
            {
                PostProcessorEnabled = gifConfig.GetValue("PostProcessorEnabled", false),
                PostProcessorCommand = gifConfig.GetValue("PostProcessorCommand", string.Empty)
            };

            IConfigurationSection jpegConfig = configuration.GetSection("JpegSettings");
            IJpegSettings jpegSettings = new JpegSettings
            {
                Quality = jpegConfig.GetValue("Quality", 75),
                PostProcessorEnabled = jpegConfig.GetValue("PostProcessorEnabled", false),
                PostProcessorCommand = jpegConfig.GetValue("PostProcessorCommand", string.Empty)
            };

            IConfigurationSection pngConfig = configuration.GetSection("PngSettings");
            IPngSettings pngSettings = new PngSettings
            {
                CompressionLevel = pngConfig.GetValue("CompressionLevel", 6),
                PostProcessorEnabled = pngConfig.GetValue("PostProcessorEnabled", false),
                PostProcessorCommand = pngConfig.GetValue("PostProcessorCommand", string.Empty)
            };

            Settings settings = new Settings(sourceRoot, destinationRoot, rateLimiterSettings, loggerSettings, gifSettings, jpegSettings, pngSettings);
            return settings;
        }
    }
}
