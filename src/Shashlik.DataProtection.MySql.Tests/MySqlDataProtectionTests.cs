using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Shashlik.Kernel.Test;
using Xunit;
using Xunit.Abstractions;
using Shouldly;

namespace Shashlik.DataProtection.MySql.Tests
{
    public class MySqlDataProtectionTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MySqlDataProtectionTests(TestWebApplicationFactory<TestStartup> factory,
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