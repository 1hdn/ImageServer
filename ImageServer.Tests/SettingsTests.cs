//using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace ImageServer.Tests
{
    public class SettingsTests
    {
        //[Fact]
        //public void TestDefaultValues()
        //{
        //    Dictionary<string, string> props = new Dictionary<string, string>();
        //    props.Add("SourceRoot", "source/root");
        //    props.Add("DestinationRoot", "destination/root");

        //    ConfigurationBuilder cb = new ConfigurationBuilder();
        //    cb.AddInMemoryCollection(props);
        //    IConfiguration config = cb.Build();

        //    ISettings settings = new Settings(config);

        //    Assert.True(settings.SourceRoot == "source/root");
        //    Assert.True(settings.DestinationRoot == "destination/root");

        //    Assert.True(settings.RateLimiterSettings.Enabled == false);
        //    Assert.True(settings.RateLimiterSettings.TimeWindowInMinutes == 1);
        //    Assert.True(settings.RateLimiterSettings.MaxRequestsInTimeWindow == 60);
        //    Assert.True(settings.RateLimiterSettings.MaxBytesInTimeWindow == 1024 * 1024 * 5);
        //    Assert.True(settings.RateLimiterSettings.Blacklist.Length == 0);
        //    Assert.True(settings.RateLimiterSettings.Whitelist.Length == 0);

        //    Assert.True(settings.LoggerSettings.WriteInfo == false);
        //    Assert.True(settings.LoggerSettings.WriteWarnings == false);
        //    Assert.True(settings.LoggerSettings.WriteErrors == false);
        //    Assert.True(settings.LoggerSettings.ToConsole == false);
        //    Assert.True(settings.LoggerSettings.ToFile == false);
        //    Assert.True(settings.LoggerSettings.Directory == string.Empty);

        //    Assert.True(settings.GifSettings.PostProcessorPath == string.Empty);
        //    Assert.True(settings.GifSettings.PostProcessorArgs == string.Empty);

        //    Assert.True(settings.JpegSettings.Quality == 75);
        //    Assert.True(settings.JpegSettings.PostProcessorPath == string.Empty);
        //    Assert.True(settings.JpegSettings.PostProcessorArgs == string.Empty);

        //    Assert.True(settings.PngSettings.CompressionLevel == 6);
        //    Assert.True(settings.PngSettings.PostProcessorPath == string.Empty);
        //    Assert.True(settings.PngSettings.PostProcessorArgs == string.Empty);
        //}

        //[Fact]
        //public void TestMinimumConfiguration()
        //{
        //    Assert.Throws<SettingsException>(() => 
        //    {
        //        ConfigurationBuilder cb = new ConfigurationBuilder();
        //        IConfiguration config = cb.Build();

        //        ISettings settings = new Settings(config);
        //    });

        //    Assert.Throws<SettingsException>(() => 
        //    {
        //        Dictionary<string, string> props = new Dictionary<string, string>();
        //        props.Add("SourceRoot", "source/root");

        //        ConfigurationBuilder cb = new ConfigurationBuilder();
        //        cb.AddInMemoryCollection(props);
        //        IConfiguration config = cb.Build();

        //        ISettings settings = new Settings(config);
        //    });

        //    Assert.Throws<SettingsException>(() =>
        //    {
        //        Dictionary<string, string> props = new Dictionary<string, string>();
        //        props.Add("DestinationRoot", "destination/root");

        //        ConfigurationBuilder cb = new ConfigurationBuilder();
        //        cb.AddInMemoryCollection(props);
        //        IConfiguration config = cb.Build();

        //        ISettings settings = new Settings(config);
        //    });
        //}

        //[Fact]
        //public void TestFullConfiguration()
        //{
        //    Dictionary<string, string> props = new Dictionary<string, string>();

        //    props.Add("SourceRoot", "source/root");
        //    props.Add("DestinationRoot", "destination/root");

        //    props.Add("RateLimiterSettings:Enabled", "true");
        //    props.Add("RateLimiterSettings:TimeWindowInMinutes", "42");
        //    props.Add("RateLimiterSettings:MaxRequestsInTimeWindow", "1337");
        //    props.Add("RateLimiterSettings:MaxBytesInTimeWindow", "1000000");
        //    props.Add("RateLimiterSettings:Whitelist:0", "1.3.3.7");
        //    props.Add("RateLimiterSettings:Blacklist:0", "6.6.6.666");
        //    props.Add("RateLimiterSettings:Blacklist:1", "666.666.666.666");

        //    props.Add("LoggerSettings:WriteInfo", "true");
        //    props.Add("LoggerSettings:WriteWarnings", "true");
        //    props.Add("LoggerSettings:WriteErrors", "true");
        //    props.Add("LoggerSettings:ToConsole", "true");
        //    props.Add("LoggerSettings:ToFile", "true");
        //    props.Add("LoggerSettings:Directory", "path/to/log");

        //    props.Add("GifSettings:PostProcessorPath", "path/to/gifsicle");
        //    props.Add("GifSettings:PostProcessorArgs", "some gifsicle args");

        //    props.Add("JpegSettings:Quality", "50");
        //    props.Add("JpegSettings:PostProcessorPath", "path/to/jpegtran");
        //    props.Add("JpegSettings:PostProcessorArgs", "some jpegtran args");

        //    props.Add("PngSettings:CompressionLevel", "3");
        //    props.Add("PngSettings:PostProcessorPath", "path/to/optipng");
        //    props.Add("PngSettings:PostProcessorArgs", "some optipng args");


        //    ConfigurationBuilder cb = new ConfigurationBuilder();
        //    cb.AddInMemoryCollection(props);
        //    IConfiguration config = cb.Build();

        //    ISettings settings = new Settings(config);

        //    Assert.True(settings.RateLimiterSettings.Enabled == true);
        //    Assert.True(settings.RateLimiterSettings.TimeWindowInMinutes == 42);
        //    Assert.True(settings.RateLimiterSettings.MaxRequestsInTimeWindow == 1337);
        //    Assert.True(settings.RateLimiterSettings.MaxBytesInTimeWindow == 1000000);
        //    Assert.True(settings.RateLimiterSettings.Whitelist.Length == 1 && settings.RateLimiterSettings.Whitelist[0] == "1.3.3.7");
        //    Assert.True(settings.RateLimiterSettings.Blacklist.Length == 2 && settings.RateLimiterSettings.Blacklist[0] == "6.6.6.666" && settings.RateLimiterSettings.Blacklist[1] == "666.666.666.666");

        //    Assert.True(settings.LoggerSettings.WriteInfo == true);
        //    Assert.True(settings.LoggerSettings.WriteWarnings == true);
        //    Assert.True(settings.LoggerSettings.WriteErrors == true);
        //    Assert.True(settings.LoggerSettings.ToConsole == true);
        //    Assert.True(settings.LoggerSettings.ToFile == true);
        //    Assert.True(settings.LoggerSettings.Directory == "path/to/log");

        //    Assert.True(settings.GifSettings.PostProcessorPath == "path/to/gifsicle");
        //    Assert.True(settings.GifSettings.PostProcessorArgs == "some gifsicle args");

        //    Assert.True(settings.JpegSettings.Quality == 50);
        //    Assert.True(settings.JpegSettings.PostProcessorPath == "path/to/jpegtran");
        //    Assert.True(settings.JpegSettings.PostProcessorArgs == "some jpegtran args");

        //    Assert.True(settings.PngSettings.CompressionLevel == 3);
        //    Assert.True(settings.PngSettings.PostProcessorPath == "path/to/optipng");
        //    Assert.True(settings.PngSettings.PostProcessorArgs == "some optipng args");
        //}


        [Fact]
        public void TestDefaultValues()
        {
            string sourceRoot = "source/root";
            Location destinationRoot = new Location(new Uri("http://site.com/images"), "/var/www/site.com/images");
   
            ISettings settings = new Settings(sourceRoot, destinationRoot);

            Assert.True(settings.SourceRoot == "source/root");
            Assert.True(settings.DestinationRoot.AbsoluteUrl == destinationRoot.AbsoluteUrl);
            Assert.True(settings.DestinationRoot.AbsolutePath == destinationRoot.AbsolutePath);

            Assert.True(settings.RateLimiterSettings.Enabled == false);
            Assert.True(settings.RateLimiterSettings.TimeWindowInMinutes == 10);
            Assert.True(settings.RateLimiterSettings.MaxRequestsInTimeWindow == 1000);
            Assert.True(settings.RateLimiterSettings.MaxBytesInTimeWindow == 1024 * 1024 * 100);
            Assert.True(settings.RateLimiterSettings.Blacklist.Length == 0);
            Assert.True(settings.RateLimiterSettings.Whitelist.Length == 0);

            Assert.True(settings.LoggerSettings.WriteInfo == false);
            Assert.True(settings.LoggerSettings.WriteWarnings == false);
            Assert.True(settings.LoggerSettings.WriteErrors == false);
            Assert.True(settings.LoggerSettings.ToConsole == false);
            Assert.True(settings.LoggerSettings.ToFile == false);
            Assert.True(settings.LoggerSettings.Directory == string.Empty);

            Assert.False(settings.GifSettings.PostProcessorEnabled);
            Assert.True(settings.GifSettings.PostProcessorCommand == string.Empty);

            Assert.True(settings.JpegSettings.Quality == 75);
            Assert.True(settings.JpegSettings.PostProcessorCommand == string.Empty);

            Assert.True(settings.PngSettings.CompressionLevel == 6);
            Assert.True(settings.PngSettings.PostProcessorCommand == string.Empty);
        }

        [Fact]
        public void TestMinimumConfiguration()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Settings settings = new Settings(null, null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Settings settings = new Settings("source/root", null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Settings settings = new Settings("", new Location(new Uri("http://site.com/images"), "/var/www/site.com/images"));
            });
        }

        [Fact]
        public void TestFullConfiguration()
        {
            string sourceRoot = "source/root";
            Location destinationRoot = new Location(new Uri("http://site.com/images"), "/var/www/site.com/images");

            RateLimiterSettings rateLimiterSettings = new RateLimiterSettings();
            rateLimiterSettings.Enabled = true;
            rateLimiterSettings.TimeWindowInMinutes = 42;
            rateLimiterSettings.MaxRequestsInTimeWindow = 1337;
            rateLimiterSettings.MaxBytesInTimeWindow = 1000000;
            rateLimiterSettings.Whitelist = new string[] { "1.3.3.7" };
            rateLimiterSettings.Blacklist = new string[] { "6.6.6.666", "666.666.666.666" };

            LoggerSettings loggerSettings = new LoggerSettings();
            loggerSettings.WriteInfo = true;
            loggerSettings.WriteWarnings = true;
            loggerSettings.WriteErrors = true;
            loggerSettings.ToConsole = true;
            loggerSettings.ToFile = true;
            loggerSettings.Directory = "path/to/log";

            GifSettings gifSettings = new GifSettings();
            gifSettings.PostProcessorEnabled = true;
            gifSettings.PostProcessorCommand = "path/to/gifsicle some gifsicle args";

            JpegSettings jpegSettings = new JpegSettings();
            jpegSettings.Quality = 50;
            jpegSettings.PostProcessorEnabled = true;
            jpegSettings.PostProcessorCommand = "path/to/jpegtran some jpegtran args";

            PngSettings pngSettings = new PngSettings();
            pngSettings.CompressionLevel = 3;
            pngSettings.PostProcessorEnabled = true;
            pngSettings.PostProcessorCommand = "path/to/optipng some optipng args";

            Settings settings = new Settings(sourceRoot, destinationRoot, rateLimiterSettings, loggerSettings, gifSettings, jpegSettings, pngSettings);

            Assert.True(settings.SourceRoot == sourceRoot);
            Assert.True(settings.DestinationRoot.AbsoluteUrl == destinationRoot.AbsoluteUrl);
            Assert.True(settings.DestinationRoot.AbsolutePath == destinationRoot.AbsolutePath);

            Assert.True(settings.RateLimiterSettings.Enabled == true);
            Assert.True(settings.RateLimiterSettings.TimeWindowInMinutes == 42);
            Assert.True(settings.RateLimiterSettings.MaxRequestsInTimeWindow == 1337);
            Assert.True(settings.RateLimiterSettings.MaxBytesInTimeWindow == 1000000);
            Assert.True(settings.RateLimiterSettings.Whitelist.Length == 1 && settings.RateLimiterSettings.Whitelist[0] == "1.3.3.7");
            Assert.True(settings.RateLimiterSettings.Blacklist.Length == 2 && settings.RateLimiterSettings.Blacklist[0] == "6.6.6.666" && settings.RateLimiterSettings.Blacklist[1] == "666.666.666.666");

            Assert.True(settings.LoggerSettings.WriteInfo == true);
            Assert.True(settings.LoggerSettings.WriteWarnings == true);
            Assert.True(settings.LoggerSettings.WriteErrors == true);
            Assert.True(settings.LoggerSettings.ToConsole == true);
            Assert.True(settings.LoggerSettings.ToFile == true);
            Assert.True(settings.LoggerSettings.Directory == "path/to/log");

            Assert.True(settings.GifSettings.PostProcessorEnabled);
            Assert.True(settings.GifSettings.PostProcessorCommand == "path/to/gifsicle some gifsicle args");

            Assert.True(settings.JpegSettings.Quality == 50);
            Assert.True(settings.JpegSettings.PostProcessorEnabled);
            Assert.True(settings.JpegSettings.PostProcessorCommand == "path/to/jpegtran some jpegtran args");

            Assert.True(settings.PngSettings.CompressionLevel == 3);
            Assert.True(settings.PngSettings.PostProcessorEnabled);
            Assert.True(settings.PngSettings.PostProcessorCommand == "path/to/optipng some optipng args");
        }
    }
}
