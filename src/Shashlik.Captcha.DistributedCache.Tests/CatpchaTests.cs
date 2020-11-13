using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Captcha.DistributedCache.Tests
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
            var securityStamp = "123";

            // 直接验证
            {
                var code = await captcha.Build("login", "13000000000", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code.Code}");
                (await captcha.IsValid("login", "13000000000", code.Code, securityStamp)).ShouldBeTrue();
                (await captcha.IsValid("login", "13000000000", code.Code, securityStamp)).ShouldBeFalse();
            }
        }

        [Fact]
        public async Task CaptchaTests2()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";


            // 一直验证
            {
                var code = await captcha.Build("login", "13000000000", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code.Code}");

                int count = 1;
                while (true)
                {
                    if (count > 5)
                        break;
                    await Task.Delay(10_000);
                    var res = (await captcha.IsValid("login", "13000000000", code.Code, securityStamp, false));
                    _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】验证结果: {res}");
                    if (count > 3)
                        res.ShouldBeFalse();
                    else
                        res.ShouldBeTrue();
                    count++;
                }
            }
        }


        [Fact]
        public async Task CaptchaTests3()
        {
            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            // 40秒后验证
            {
                var code = await captcha.Build("login", "13000000000", securityStamp);
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code.Code}");

                await Task.Delay(40_000);
                (await captcha.IsValid("login", "13000000000", code.Code, securityStamp)).ShouldBeFalse();
            }
        }
    }
}