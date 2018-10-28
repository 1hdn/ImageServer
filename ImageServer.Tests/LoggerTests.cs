using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ImageServer.Tests
{
    public class LoggerTests
    {
        const string _infoMessage = "This is information";
        const string _warningMessage = "This is a warning";
        const string _errorMessage = "This is an error";
        readonly string _logdir = Path.Combine(AppContext.BaseDirectory, "testlogs");
        static object _lock = new object();

        [Fact]
        public void TestInfoToConsole()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteInfo = true, ToConsole = true });
            string logValue = GetConsoleLog(logger);
            Assert.Contains(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }

        [Fact]
        public void TestWarningToConsole()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteWarnings = true, ToConsole = true });
            string logValue = GetConsoleLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.Contains(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }

        [Fact]
        public void TestErrorToConsole()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteErrors = true, ToConsole = true });
            string logValue = GetConsoleLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.Contains(_errorMessage, logValue);
        }

        [Fact]
        public void TestNothingToConsole()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteInfo = true, WriteWarnings = true, WriteErrors = true });
            string logValue = GetConsoleLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }

        [Fact]
        public void TestInfoToFile()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteInfo = true, ToFile = true, Directory = _logdir });
            string logValue = GetFileLog(logger);
            Assert.Contains(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }

        [Fact]
        public void TestWarningToFile()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteWarnings = true, ToFile = true, Directory = _logdir });
            string logValue = GetFileLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.Contains(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }

        [Fact]
        public void TestErrorToFile()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteErrors = true, ToFile = true, Directory = _logdir });
            string logValue = GetFileLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.Contains(_errorMessage, logValue);
        }

        [Fact]
        public void TestNothingToFile()
        {
            Logger logger = new Logger(new LoggerSettings() { WriteInfo = true, WriteWarnings = true, WriteErrors = true });
            string logValue = GetFileLog(logger);
            Assert.DoesNotContain(_infoMessage, logValue);
            Assert.DoesNotContain(_warningMessage, logValue);
            Assert.DoesNotContain(_errorMessage, logValue);
        }


        private string GetConsoleLog(ILogger logger)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(stringBuilder))
            {
                Console.SetOut(textWriter);
                logger.LogInfo(_infoMessage);
                logger.LogWarning(_warningMessage);
                logger.LogError(_errorMessage);
                textWriter.Flush();
            }
            return stringBuilder.ToString();
        }

        private string GetFileLog(ILogger logger)
        {
            string text = string.Empty;

            lock (_lock)
            {
                if (Directory.Exists(_logdir))
                {
                    Directory.Delete(_logdir, true);
                }

                logger.LogInfo(_infoMessage);
                logger.LogWarning(_warningMessage);
                logger.LogError(_errorMessage);

                if (Directory.Exists(_logdir))
                {
                    DirectoryInfo di = new DirectoryInfo(_logdir);
                    FileInfo[] fi = di.GetFiles();
                    if (fi.Length > 0)
                    {
                        text = File.ReadAllText(fi[0].FullName);
                    }
                }
            }

            return text;
        }
    }
}
