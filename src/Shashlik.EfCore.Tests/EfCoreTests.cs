using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore.Tests;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Captcha.Tests
{
    public class EfCoreTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EfCoreTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task IntegrationTest()
        {
            // var testManager = GetService<TestManager>();
            // await testManager.CreateUser(Guid.NewGuid().ToString(), new[] {"add_user_test_role"});
        }
    }
}