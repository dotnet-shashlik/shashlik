using System.Threading;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Sms.Limit.DistributedCache.Tests
{
    public class CacheLimitTests : KernelTestBase
    {
        public CacheLimitTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
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
            smsLimit.ShouldBeOfType(typeof(DistributedCacheSmsLimit));

            // 先清除数据
            {
                var keys = RedisHelper.Keys($"SMS_CACHE_LIMIT:*");
                foreach (var key in keys)
                {
                    RedisHelper.Del(key);
                }
            }

            {
                smsLimit.CanSend(_testPhone1, _subject).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2, _subject).ShouldBeTrue();

                smsLimit.SendDone(_testPhone1, _subject);
                smsLimit.SendDone(_testPhone2, _subject);

                smsLimit.CanSend(_testPhone1, _subject).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2, _subject).ShouldBeFalse();
                smsLimit.CanSend(_testPhone1, _subject).ShouldBeFalse();
                smsLimit.CanSend(_testPhone2, _subject).ShouldBeFalse();

                Thread.Sleep(61 * 1000);
                smsLimit.CanSend(_testPhone1, _subject).ShouldBeTrue();
                smsLimit.CanSend(_testPhone2, _subject).ShouldBeTrue();
            }
        }
    }
}