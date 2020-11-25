using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Shashlik.Kernel.Test;
using Shashlik.Sms.Exceptions;
using Shashlik.Utils.Extensions;
using Xunit;
using Xunit.Abstractions;
using Shouldly;

namespace Shashlik.Sms.Cap.Tests
{
    public class SmsCapTests : KernelTestBase
    {
        public SmsCapTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
        }

        private readonly string _testPhone1 = "13000000001";
        private readonly string _testPhone2 = "13000000002";
        private const string CachePrefix = "SMS_LIMIT:{0}:{1}";

        [Fact]
        public void IntegrationTest()
        {
            var sms = GetService<ISms>();
            var smsSender = GetService<ISmsSender>();
            var smsLimit = GetService<ISmsLimit>();
            smsLimit.GetType().ShouldBe(typeof(MemorySmsLimit));
            var cache = GetService<IDistributedCache>();
            cache.Remove(CachePrefix.Format(_testPhone1, "Login"));
            cache.Remove(CachePrefix.Format(_testPhone2, "Login"));

            {
                Should.Throw<SmsArgException>(() => sms.Send("error", "Login"));
                Should.Throw<SmsArgException>(() => sms.Send(new[] {"error", _testPhone1}, "Login"));
                Should.Throw<SmsOptionsException>(() => sms.Send(_testPhone1, "error"));
                Should.Throw<SmsOptionsException>(() => sms.Send(new[] {_testPhone1, _testPhone2}, "error"));
            }

            {
                sms.Send(_testPhone1, "Login");
                Should.Throw<SmsLimitException>(() => sms.Send(_testPhone1, "Login"));
                Should.Throw<SmsLimitException>(() => sms.Send(_testPhone1, "Login"));
                sms.Send(new[] {_testPhone1, _testPhone2}, "Login");
                Thread.Sleep(60 * 1000);
                sms.Send(_testPhone1, "Login");
            }

            {
                smsSender.Send(_testPhone2, "Login");
                DateTime start = DateTime.Now;
                while ((DateTime.Now - start).TotalMinutes <= 5)
                {
                    if (SendSmsEventForTestHandler.HasSend)
                        break;

                    Task.Delay(3000);
                }

                if (!SendSmsEventForTestHandler.HasSend)
                    throw new Exception();
            }
        }
    }
}