using System.Threading;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Sms.Options;
using Microsoft.Extensions.Caching.Memory;

namespace Shashlik.Sms.Limit.Redis.Tests
{
    public class RedisLimitTests : KernelTestBase
    {
        public RedisLimitTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
        }

        private readonly string _testPhone1 = "13000000001";
        private readonly string _testPhone2 = "13000000002";
        private readonly string _subject = "Login";

        [Fact]
        public void IntegrationTest()
        {
            var smsLimit = GetService<ISmsLimit>();
            smsLimit.ShouldBeOfType(typeof(RedisSmsLimit));

            // 先清除数据
            {
                var keys = RedisHelper.Keys($"SMS_REDIS_LIMIT:*");
                foreach (var key in keys)
                {
                    RedisHelper.Del(key);
                }
            }

            {
                smsLimit.CanSend(_testPhone1).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2).ShouldBeTrue();

                smsLimit.SendDone(_testPhone1);
                smsLimit.SendDone(_testPhone2);

                smsLimit.CanSend(_testPhone1).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2).ShouldBeFalse();
                smsLimit.CanSend(_testPhone1).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2).ShouldBeFalse();

                Thread.Sleep(61 * 1000);
                smsLimit.CanSend(_testPhone1).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2).ShouldBeTrue();
            }
        }

        [Fact]
        public void MemoryLimitTest()
        {
            var options = GetService<IOptionsMonitor<SmsOptions>>();
            var smsLimit = new MemorySmsLimit(GetService<IMemoryCache>(), options);

            {
                smsLimit.CanSend(_testPhone1).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2).ShouldBeTrue();

                smsLimit.SendDone(_testPhone1);
                smsLimit.SendDone(_testPhone2);

                smsLimit.CanSend(_testPhone1).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2).ShouldBeFalse();
                smsLimit.CanSend(_testPhone1).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2).ShouldBeFalse();

                Thread.Sleep(61 * 1000);
                smsLimit.CanSend(_testPhone1).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2).ShouldBeTrue();
            }
        }
    }
}