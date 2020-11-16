using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.DataProtection.Redis.Tests
{
    public class PostgreSqlDataProtectionTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PostgreSqlDataProtectionTests(TestWebApplicationFactory<TestStartup> factory,
            ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Tests()
        {
            var dataProtectionProvider = GetService<IDataProtectionProvider>();

            var s1 = dataProtectionProvider.CreateProtector("test")
                .Protect("123");

            dataProtectionProvider.CreateProtector("test")
                .Unprotect(s1)
                .ShouldBe("123");

            await Task.CompletedTask;
        }
    }
}