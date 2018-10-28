//using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ImageServer.Tests
{
    public class RateLimiterTests
    {
        [Fact]
        public void TestBlackList()
        {
            RateLimiter rateLimiter = new RateLimiter(GetRateLimiterSettings(true));
            Assert.False(rateLimiter.IsBlacklisted("111.111.111.111"));
            Assert.True(rateLimiter.IsBlacklisted("888.888.888.888"));
        }

        [Fact]
        public void TestMaxRequests()
        {
            RateLimiter rateLimiter = new RateLimiter(GetRateLimiterSettings(true));

            string ip = "123.123.123.123";
            rateLimiter.RegisterRequest(ip, 1);
            Assert.False(rateLimiter.IsLimitExceeded(ip));
            rateLimiter.RegisterRequest(ip, 1);
            Assert.False(rateLimiter.IsLimitExceeded(ip));
            rateLimiter.RegisterRequest(ip, 1);
            Assert.True(rateLimiter.IsLimitExceeded(ip));

            string whitelistIp = "111.111.111.111";
            rateLimiter.RegisterRequest(whitelistIp, 1);
            Assert.False(rateLimiter.IsLimitExceeded(whitelistIp));
            rateLimiter.RegisterRequest(whitelistIp, 1);
            Assert.False(rateLimiter.IsLimitExceeded(whitelistIp));
            rateLimiter.RegisterRequest(whitelistIp, 1);
            Assert.False(rateLimiter.IsLimitExceeded(whitelistIp));
        }

        [Fact]
        public void TestMaxBytes()
        {
            RateLimiter rateLimiter = new RateLimiter(GetRateLimiterSettings(true));

            string ip = "234.234.234.234";
            rateLimiter.RegisterRequest(ip, 750);
            Assert.False(rateLimiter.IsLimitExceeded(ip));
            rateLimiter.RegisterRequest(ip, 750);
            Assert.True(rateLimiter.IsLimitExceeded(ip));

            string whitelistIp = "222.222.222.222";
            rateLimiter.RegisterRequest(whitelistIp, 750);
            Assert.False(rateLimiter.IsLimitExceeded(whitelistIp));
            rateLimiter.RegisterRequest(whitelistIp, 750);
            Assert.False(rateLimiter.IsLimitExceeded(whitelistIp));
        }

        private IRateLimiterSettings GetRateLimiterSettings(bool enabled)
        {
            RateLimiterSettings rateLimiterSettings = new RateLimiterSettings
            {
                Enabled = enabled,
                TimeWindowInMinutes = 1,
                MaxRequestsInTimeWindow = 2,
                MaxBytesInTimeWindow = 1000,
                Whitelist = new string[] { "000.000.000.000", "111.111.111.111", "222.222.222.222" },
                Blacklist = new string[] { "777.777.777.777", "888.888.888.888", "999.999.999.999" }
            };
            return rateLimiterSettings;
        }
    }
}
