using System;
using System.Threading.Tasks;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Captcha.Redis.Tests
{
    public class CatpchaTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CatpchaTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task CaptchaTests1()
        {
            var captcha = GetService<ICaptcha>();
            captcha.ShouldBeOfType<RedisCacheCatpcha>();
            var securityStamp = "123";

            // 直接验证
            {
                var code = await captcha.Build("login", "13000000000", securityStamp: securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code}");
                (await captcha.IsValid("login", "13000000000", code, securityStamp)).ShouldBeTrue();
                (await captcha.IsValid("login", "13000000000", code, securityStamp)).ShouldBeFalse();
            }
        }

        [Fact]
        public async Task CaptchaTests2()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";


            // 一直验证
            {
                var code = await captcha.Build("login", "13000000001", lifeTimeSeconds: 9, securityStamp: securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code}");


                var res = (await captcha.IsValid("login", "13000000001", code, securityStamp, false));
                res.ShouldBeTrue();
                await Task.Delay(3000);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】验证结果: {res}");
                res = (await captcha.IsValid("login", "13000000001", code, securityStamp, false));
                res.ShouldBeTrue();
                await Task.Delay(3000);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】验证结果: {res}");
                res = (await captcha.IsValid("login", "13000000001", code, securityStamp, false));
                res.ShouldBeTrue();
                await Task.Delay(2000);
                res = (await captcha.IsValid("login", "13000000001", code, securityStamp, false));
                res.ShouldBeTrue();
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】验证结果: {res}");
                await Task.Delay(3000);
                res = (await captcha.IsValid("login", "13000000001", code, securityStamp, false));
                res.ShouldBeFalse();
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】验证结果: {res}");

            }
        }


        [Fact]
        public async Task CaptchaTests3()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            // 40秒后验证
            {
                var code = await captcha.Build("login", "13000000002", securityStamp: securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code}");

                await Task.Delay(40_000);
                (await captcha.IsValid("login", "13000000000", code, securityStamp)).ShouldBeFalse();
            }
        }

        [Fact]
        public async Task CaptchaTests4()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            // 40秒后验证
            {
                await captcha.Build("login", "13000000003", 30, 3, "123123", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {123123}");
                (await captcha.IsValid("login", "13000000003", "error", securityStamp)).ShouldBeFalse();
                (await captcha.IsValid("login", "13000000003", "error", securityStamp)).ShouldBeFalse();
                (await captcha.IsValid("login", "13000000003", "error", securityStamp)).ShouldBeFalse();
                (await captcha.IsValid("login", "13000000003", "123123", securityStamp)).ShouldBeFalse();
            }
        }

        [Fact]
        public async Task CaptchaTests5()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            {
                await captcha.Build("login", "13000000004", 30, 3, "123123", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {123123}");
                (await captcha.IsValid("login", "13000000004", "error", securityStamp)).ShouldBeFalse();
                (await captcha.IsValid("login", "13000000004", "error", securityStamp)).ShouldBeFalse();
                (await captcha.IsValid("login", "13000000004", "123123", securityStamp)).ShouldBeTrue();
                (await captcha.IsValid("login", "13000000004", "123123", securityStamp)).ShouldBeFalse();
            }
        }

        [Fact]
        public async Task CaptchaTests6()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            {
                await captcha.Build("login", "13000000005", 30, 3, "123123", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {123123}");
                (await captcha.IsValid("login", "13000000005", "123123", securityStamp, false)).ShouldBeTrue();
                (await captcha.IsValid("login", "13000000005", "123123", securityStamp)).ShouldBeTrue();
                (await captcha.IsValid("login", "13000000005", "123123", securityStamp)).ShouldBeFalse();
            }
        }
    }
}