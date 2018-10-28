using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ImageServer.Tests")]
namespace ImageServer
{
    internal class RateLimiter
    {
        private static object _lock = new object();
        private static Dictionary<string, List<RequestInfo>> _requests = new Dictionary<string, List<RequestInfo>>();
        private static DateTime _lastCleanUp = DateTime.Now;
        private IRateLimiterSettings _settings;
        private ILogger _logger;

        private struct RequestInfo
        {
            public DateTime Time { get; }
            public long ByteSize { get; }

            public RequestInfo(long byteSize)
            {
                Time = DateTime.Now;
                ByteSize = byteSize;
            }
        }

        public RateLimiter(IRateLimiterSettings settings) : this(settings, new Logger(new LoggerSettings()))
        {
        }

        public RateLimiter(IRateLimiterSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public bool IsBlacklisted(string ipAddress)
        {
            if (!_settings.Enabled)
            {
                return false;
            }

            if (_settings.Blacklist.Contains(ipAddress))
            {
                _logger.LogInfo($"Request from blacklisted ip address ({ipAddress}).");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLimitExceeded(string ipAddress)
        {
            if (!_settings.Enabled || _settings.Whitelist.Contains(ipAddress))
            {
                return false;
            }

            RequestInfo[] requestsFromIp = null;

            lock (_lock)
            {
                if (_requests.ContainsKey(ipAddress))
                {
                    requestsFromIp = _requests[ipAddress].ToArray();
                }
            }

            if (requestsFromIp == null)
            {
                return false;
            }

            RequestInfo[] activeRequests = requestsFromIp.Where(x => DateTime.Now.Subtract(x.Time).TotalMinutes <= _settings.TimeWindowInMinutes).ToArray();

            int requestCount = activeRequests.Length;
            if (requestCount > _settings.MaxRequestsInTimeWindow)
            {
                _logger.LogInfo($"Request from {ipAddress} exceeds the request limit ({requestCount} active requests - limit is {_settings.MaxRequestsInTimeWindow}) requests.");
                return true;
            }

            long byteCount = activeRequests.Sum(x => x.ByteSize);
            if (byteCount > _settings.MaxBytesInTimeWindow)
            {
                _logger.LogInfo($"Request from {ipAddress} exceeds the byte limit ({byteCount} bytes processed - limit is {_settings.MaxBytesInTimeWindow}) bytes.");
                return true;
            }

            return false;
        }

        public void RegisterRequest(string ipAddress, long byteSize)
        {
            if (!_settings.Enabled)
            {
                return;
            }

            lock (_lock)
            {
                if (_requests.ContainsKey(ipAddress))
                {
                    _requests[ipAddress].Add(new RequestInfo(byteSize));
                }
                else
                {
                    _requests.Add(ipAddress, new List<RequestInfo>() { new RequestInfo(byteSize) });
                }

                if (ShouldCleanUp())
                {
                    DoCleanUp();
                }
            }
        }

        private bool ShouldCleanUp()
        {
            double minutesSinceLastCleanUp = DateTime.Now.Subtract(_lastCleanUp).TotalMinutes;
            return minutesSinceLastCleanUp > _settings.TimeWindowInMinutes;
        }

        private void DoCleanUp()
        {
            DateTime now = DateTime.Now;

            List<string> keys = _requests.Keys.ToList();
            foreach (string key in keys)
            {
                double minutesSinceLastRequest = _requests[key].Min(x => now.Subtract(x.Time).TotalMinutes);
                if (minutesSinceLastRequest > _settings.TimeWindowInMinutes)
                {
                    _requests.Remove(key);
                }
                else
                {
                    _requests[key] = _requests[key].Where(x => now.Subtract(x.Time).TotalMinutes <= _settings.TimeWindowInMinutes).ToList();
                }
            }

            _lastCleanUp = DateTime.Now;
        }
    }
}
