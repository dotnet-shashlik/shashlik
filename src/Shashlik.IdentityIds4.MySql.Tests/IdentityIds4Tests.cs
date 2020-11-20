using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Shashlik.Identity;
using Shashlik.Kernel.Test;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.IdentityIds4.MySql.Tests
{
    public class IdentityIds4Tests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _testRoleName = Guid.NewGuid().ToString("n");
        private readonly string _testUserName = Guid.NewGuid().ToString("n");
        private readonly string _testUserMail = "280780363@qq.com";
        private readonly string _testUserPhone = "13000000000";
        private readonly string _testUserIdCard = "123456789012345678";
        private readonly string _password = Guid.NewGuid().ToString("n");
        private readonly string _clientId = "test_client_id";
        private readonly string _apiScope = "test_api";

        private readonly IShashlikUserManager _userManager;
        private readonly IShashlikRoleManager _roleManager;

        public IdentityIds4Tests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;

            _userManager = GetService<IShashlikUserManager>();
            _roleManager = GetService<IShashlikRoleManager>();
        }

        [Fact]
        public async Task IntegrationTest()
        {
            var dbContext = GetService<ShashlikIdentityDbContext>();
            // 清除数据
            {
                dbContext.Set<IdentityUserRole<int>>().RemoveRange(dbContext.Set<IdentityUserRole<int>>().ToList());
                dbContext.Set<Roles>().RemoveRange(dbContext.Set<Roles>().ToList());
                dbContext.Set<Users>().RemoveRange(dbContext.Set<Users>().ToList());
                await dbContext.SaveChangesAsync();
            }

            // 等待5秒,等待数据库迁移
            Thread.Sleep(5000);

            Users user;

            // 初始化测试数据
            {
                _roleManager.CreateAsync(new Roles
                {
                    Name = _testRoleName
                }).GetAwaiter().GetResult();
                user = new Users
                {
                    UserName = _testUserName,
                    Email = _testUserMail,
                    EmailConfirmed = true,
                    PhoneNumber = _testUserPhone,
                    PhoneNumberConfirmed = true,
                    IdCard = _testUserIdCard,
                    TwoFactorEnabled = true,
                    LockoutEnabled = true
                };


                var res = _userManager.CreateAsync(user, _password).GetAwaiter().GetResult();
                res.Succeeded.ShouldBeTrue();
                res = _userManager.AddToRoleAsync(user, _testRoleName).GetAwaiter().GetResult();
                res.Succeeded.ShouldBeTrue();
            }

            // 登录验证码测试
            {
                var captcha = await _userManager.GenerateLoginCaptcha(user);
                captcha.IsNullOrWhiteSpace().ShouldBeFalse();
                (await _userManager.IsValidLoginCaptcha(user, captcha)).ShouldBeTrue();
                (await _userManager.IsValidLoginCaptcha(user, "absolute_error")).ShouldBeFalse();
            }

            // 密码登录测试
            {
                for (int i = 0; i < 10; i++)
                {
                    var res = await _userManager.CheckPasswordSignInAsync(user, Guid.NewGuid().ToString("n"), true);
                    res.Succeeded.ShouldBeFalse();
                    await _userManager.AccessFailedAsync(user);
                    Thread.Sleep(10);
                }

                // 正确的密码校验,被锁定
                var res1 = (await _userManager.CheckPasswordSignInAsync(user, _password, false));
                res1.Succeeded.ShouldBeFalse();
                res1.ShouldBe(SignInResult.LockedOut);

                // 清除锁定时间
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                // 清除失败次数
                (await _userManager.ResetAccessFailedCountAsync(user)).Succeeded.ShouldBeTrue();

                // 正确的密码校验
                var res2 = (await _userManager.CheckPasswordSignInAsync(user, _password, false));
                res2.ShouldBe(SignInResult.TwoFactorRequired);
            }

            // 身份证号码查找测试
            {
                (await _userManager.FindIdentityUserByIdCardAsync(_testUserIdCard)).IdCard.ShouldBe(_testUserIdCard);
            }

            // 手机号码查找des
            {
                (await _userManager.FindIdentityUserByPhoneNumberAsync(_testUserPhone)).PhoneNumber.ShouldBe(_testUserPhone);
            }

            // totp 二维码生成
            {
                var qrcode = await _userManager.GetTotpTwoFactorQrcode(user.IdString);
                _testOutputHelper.WriteLine(qrcode);
                qrcode.StartsWith("otpauth://").ShouldBeTrue();
            }

            // ids4 captcha登录
            {
                var captcha = await _userManager.GenerateLoginCaptcha(user);
                captcha.IsNullOrWhiteSpace().ShouldBeFalse();

                var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"client_id", _clientId},
                    {"grant_type", "captcha"},
                    {"scope", _apiScope},
                    {"identity", _testUserPhone},
                    {"captcha", captcha},
                });

                var res = await HttpClient.PostAsync("/connect/token", content);
                res.IsSuccessStatusCode.ShouldBeTrue();
                var jObject = (await res.Content.ReadAsStringAsync()).DeserializeJson<JObject>();
                jObject["access_token"]?.Value<string>().IsNullOrWhiteSpace().ShouldBeFalse();
            }

            // password/two_factor 登录
            {
                // 第一步密码登录
                var content1 = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"client_id", _clientId},
                    {"grant_type", "password"},
                    {"scope", _apiScope},
                    {"username", _testUserName},
                    {"password", _password},
                });

                var res1 = await HttpClient.PostAsync("/connect/token", content1);
                res1.IsSuccessStatusCode.ShouldBeFalse();
                var jObject1 = (await res1.Content.ReadAsStringAsync()).DeserializeJson<JObject>();
                jObject1["code"]?.Value<string>().ShouldBe("202");
                var security = jObject1["security"]?.Value<string>();
                security.ShouldNotBeNullOrWhiteSpace();

                // 发送验证码
                var captcha = await _userManager.GenerateLoginCaptcha(user);
                captcha.IsNullOrWhiteSpace().ShouldBeFalse();

                // 第二步两步验证
                var content2 = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"client_id", _clientId},
                    {"grant_type", "password"},
                    {"scope", _apiScope},
                    {"token", captcha},
                    {"security", security},
                    {"provider", "Captcha"},
                });

                var res2 = await HttpClient.PostAsync("/connect/token", content2);
                res2.IsSuccessStatusCode.ShouldBeTrue();
                var jObject2 = (await res1.Content.ReadAsStringAsync()).DeserializeJson<JObject>();
                jObject2["access_token"]?.Value<string>().IsNullOrWhiteSpace().ShouldBeFalse();
            }
        }
    }
}