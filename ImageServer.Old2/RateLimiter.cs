using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageServer
{
    internal class RateLimiter : IRateLimiter
    {
        private static object _lock = new object();
        private static Dictionary<string, List<RequestInfo>> _requests = new Dictionary<string, List<RequestInfo>>();
        private static DateTime _lastCleanUp = DateTime.Now;
        private const int _cleanUpIntervalInMinutes = 10;
        private IRateLimiterSettings _settings;

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

        public RateLimiter(IRateLimiterSettings settings)
        {
            _settings = settings;
        }

        public bool IsBlacklisted(string ipAddress)
        {
            if (!_settings.Enabled)
            {
                return false;
            }

            return _settings.Blacklist.Contains(ipAddress);
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

            if (activeRequests.Length > _settings.MaxRequestsInTimeWindow)
            {
                return true;
            }

            if (activeRequests.Sum(x => x.ByteSize) > _settings.MaxBytesInTimeWindow)
            {
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
            return minutesSinceLastCleanUp > _cleanUpIntervalInMinutes;
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
