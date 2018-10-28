using System;
using System.IO;

namespace ImageServer
{
    public class Logger : ILogger
    {
        private static object _lock = new object();

        private enum LogType
        {
            Info,
            Warning,
            Error
        }

        ILoggerSettings _settings;

        public Logger(ILoggerSettings settings)
        {
            _settings = settings;
        }

        public void LogInfo(string message)
        {
            if (_settings.WriteInfo)
            {
                Log(message, LogType.Info);
            }
        }

        public void LogWarning(string message)
        {
            if (_settings.WriteWarnings)
            {
                Log(message, LogType.Warning);
            }
        }

        public void LogError(string message)
        {
            if (_settings.WriteErrors)
            {
                Log(message, LogType.Error);
            }
        }

        private void Log(string message, LogType logType)
        {
            DateTime now = DateTime.Now;
            string line = $"{now.ToString("yyyy-MM-dd HH:mm:ss")}: {logType.ToString().PadRight(7)} => {message}{Environment.NewLine}";

            if (_settings.ToConsole)
            {
                Console.Write(line);
            }

            if (_settings.ToFile)
            {
                string filename = $"{now.ToString("yyyyMMdd")}.log";
                string directory = _settings.Directory;
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Environment.CurrentDirectory;
                }
                lock (_lock)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string filepath = Path.Combine(directory, filename);
                    File.AppendAllText(filepath, line);
                }
            }
        }
    }
}
