using System.Threading;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Options;
using Shashlik.Sms.Options;
using Microsoft.Extensions.Caching.Memory;
using Shashlik.Sms.Aliyun;
using System;
using Microsoft.Extensions.Logging;
using Shashlik.Sms.Exceptions;

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
        private readonly string _captchaSubject = "Captcha";

        [Fact]
        public void RedisLimitTest()
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

            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeTrue();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeTrue();

            smsLimit.SendDone(_testPhone1, _captchaSubject);
            smsLimit.SendDone(_testPhone2, _captchaSubject);
            smsLimit.SendDone(_testPhone1, _captchaSubject);
            smsLimit.SendDone(_testPhone2, _captchaSubject);

            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeFalse();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeFalse();

            Thread.Sleep(61 * 1000);
            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeTrue();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeTrue();
        }

        [Fact]
        public void MemoryLimitTest()
        {
            var options = GetService<IOptionsMonitor<SmsOptions>>();
            var smsLimit = new MemorySmsLimit(GetService<IMemoryCache>(), options);

            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeTrue();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeTrue();

            smsLimit.SendDone(_testPhone1, _captchaSubject);
            smsLimit.SendDone(_testPhone2, _captchaSubject);
            smsLimit.SendDone(_testPhone1, _captchaSubject);
            smsLimit.SendDone(_testPhone2, _captchaSubject);

            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeFalse();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeFalse();

            Thread.Sleep(61 * 1000);
            smsLimit.CanSend(_testPhone1, _captchaSubject).ShouldBeTrue();
            smsLimit.CanSend(_testPhone2, _captchaSubject).ShouldBeTrue();
        }

        [Fact]
        public void EmptySmsTest()
        {
            var emptySmsSender = new EmptySmsSender(
                GetService<ISmsLimit>(),
                GetService<IOptionsMonitor<SmsOptions>>(),
                GetService<ILogger<EmptySmsSender>>()
            );
            emptySmsSender.SendCheck(_testPhone1, _captchaSubject);
            Should.Throw<SmsTemplateException>(() => emptySmsSender.SendCheck(_testPhone1, "none"));
            emptySmsSender.SendAsync(new[] { _testPhone1 }, _captchaSubject);
        }

        [Fact]
        public void AliyunSmsTest()
        {
            var sender = new AliyunSmsSender(
                GetService<IOptions<AliyunSmsOptions>>(),
                GetService<ISmsLimit>(),
                GetService<IOptionsMonitor<SmsOptions>>()
            );

            Should.Throw<SmsTemplateException>(async () => await sender.SendAsync(new[] { _testPhone1 }, "none"));
            Should.Throw<ArgumentException>(async () => await sender.SendAsync(new[] { _testPhone1 }, "Captcha"));
            Should.Throw<SmsTemplateException>(async () => await sender.SendAsync(new[] { _testPhone1 }, "Notify"));
            //sender.SendAsync(_testPhone1, "Captcha", "123123").GetAwaiter().GetResult();
        }

        [Fact]
        public void TClouadSmsTest()
        {
            var sender = new TCloud.TCloudSmsSender(
                GetService<IOptions<TCloud.TCloudSmsOptions>>(),
                GetService<ISmsLimit>(),
                GetService<IOptionsMonitor<SmsOptions>>()
            );

            Should.Throw<SmsTemplateException>(async () => await sender.SendAsync(new[] { _testPhone1 }, "none"));
            Should.Throw<SmsTemplateException>(async () => await sender.SendAsync(new[] { _testPhone1 }, "Notify"));
            //sender.SendAsync(_testPhone1, "Captcha2", "123123", "5").GetAwaiter().GetResult();
        }
    }
}