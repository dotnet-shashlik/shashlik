using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Shashlik.Kernel.Test;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.DataProtection.PostgreSql.Tests
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

            await Task.CompletedTask;
        }
    }
}