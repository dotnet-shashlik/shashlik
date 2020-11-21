using System.Threading.Tasks;
using Shashlik.Kernel.Test;
using Shashlik.Sms.Exceptions;
using Xunit;
using Xunit.Abstractions;
using Shouldly;

namespace Shashlik.Sms.Cap.Tests
{
    public class SmsCapTests : KernelTestBase
    {
        public SmsCapTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
        }

        private readonly string _testPhone = "13000000000";

        [Fact]
        public async Task IntegrationTest()
        {
            var sms = GetService<ISms>();
            {
                sms.Send(_testPhone, "test");
                Should.Throw<SmsArgException>(() => sms.Send(_testPhone, "test"));
            }
        }
    }
}