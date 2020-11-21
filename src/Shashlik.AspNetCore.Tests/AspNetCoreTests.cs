using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shashlik.Kernel.Test;
using Shashlik.Response;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.AspNetCore.Tests
{
    public class AspNetCoreTests : KernelTestBase
    {
        public AspNetCoreTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
        }

        [Fact]
        public async Task IntegrationTest()
        {
            {
                var res = await HttpClient.PostAsync("/Test/api1", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeTrue();
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(1);
                model.Data!.ToString().ShouldBe("api1");
            }

            {
                var res = await HttpClient.PostAsync("/Test/api2", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeTrue();
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(500);
            }

            {
                var res = await HttpClient.PostAsync("/Test/api3", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeTrue();
                var json = await res.Content.ReadAsStringAsync();
                json = json.DeserializeJson<string>();
                json.ShouldBe("api3");
            }

            {
                var res = await HttpClient.PostAsync("/Test/api4", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeTrue();
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(500);
            }

            {
                var res = await HttpClient.PostAsync("/Test/api5", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeFalse();
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(400);
                model.Success.ShouldBeFalse();
                model.Msg!.Contains("name_null").ShouldBeTrue();
                model.Msg!.Contains("age:1-18").ShouldBeTrue();
            }

            {
                var res = await HttpClient.PostAsync("/Test/api6", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(400);
            }

            {
                var res = await HttpClient.PostAsync("/Test/api7", new StringContent("{}", Encoding.UTF8, "application/json"));
                res.IsSuccessStatusCode.ShouldBeTrue();
                var json = await res.Content.ReadAsStringAsync();
                var model = json.DeserializeJson<ResponseResult>();
                model.Code.ShouldBe(400);
            }
        }
    }
}