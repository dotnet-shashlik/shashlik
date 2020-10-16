using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Captcha.Tests
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
        public async Task TotpTests()
        {
            var dataProtectionProvider = GetService<IDataProtectionProvider>();

            var s1 = dataProtectionProvider.CreateProtector("captcha")
                .Protect("123");
            var s2 = dataProtectionProvider.CreateProtector("captcha")
                .Protect("123");

            dataProtectionProvider.CreateProtector("captcha")
                .Unprotect(s1).ShouldBe("123");
            dataProtectionProvider.CreateProtector("captcha")
                .Unprotect(s2).ShouldBe("123");

            var options = GetService<IOptions<CaptchaOptions>>();
            _testOutputHelper.WriteLine($"totp容忍时间: {options.Value.LifeTimeSecond}秒.");

            var captcha = GetService<ICaptcha>();
            var securityStamp = "123";

            var code = await captcha.Build("login", "13000000000", securityStamp);
            _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】 {code.Code}");
            (await captcha.IsValid("login", "13000000000", code.Code, securityStamp)).ShouldBeTrue();

            int count = 1;
            while (true)
            {
                if (count > 18)
                    break;
                await Task.Delay(10_000);
                var res = (await captcha.IsValid("login", "13000000000", code.Code, securityStamp));
                _testOutputHelper.WriteLine($"【{DateTime.Now:HH:mm:ss}】{count * 10}秒后验证: {res}");
                //if (count > 6)
                  //  res.ShouldBeFalse();
                count++;
            }
        }
    }
}