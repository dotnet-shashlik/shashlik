using System.IO;
using System.Text;
using System.Threading.Tasks;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class StreamTests
    {
        [Fact]
        public void Tests()
        {
            var str = "testStr中文";
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var ms = new MemoryStream(bytes))
            {
                ms.GetMD5Hash().ShouldBe(HashHelper.MD5(str));
            }

            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(3, SeekOrigin.Begin);
                ms.ReadToString(Encoding.UTF8).ShouldBe(str);
            }

            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(3, SeekOrigin.Begin);
                ms.ReadAll().ShouldBe(bytes);
            }
        }

        [Fact]
        public async Task AsyncTests()
        {
            var str = "testStr中文";
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(3, SeekOrigin.Begin);
                (await ms.ReadToStringAsync(Encoding.UTF8)).ShouldBe(str);
            }

            using (var ms = new MemoryStream(bytes))
            {
                ms.Seek(3, SeekOrigin.Begin);
                (await ms.ReadAllAsync()).ShouldBe(bytes);
            }
        }
    }
}