using System;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    public class HashHelperTests
    {
        private const string text = "text";
        private const string secret = "secret";
        
        [Fact]
        public void Test()
        {
            Should.Throw<Exception>(() => HashHelper.MD5(null));
            
            HashHelper.MD5(text).ShouldBe("1cb251ec0d568de6a929b520c4aed8d1");
            HashHelper.SHA1(text).ShouldBe("372ea08cab33e71c02c651dbc83a474d32c676ea");
            HashHelper.SHA256(text).ShouldBe("982d9e3eb996f559e633f4d194def3761d909f5a3b647d1a851fead67c32c9d1");
            HashHelper.SHA512(text).ShouldBe("eaf2c12742cb8c161bcbd84b032b9bb98999a23282542672ca01cc6edd268f7dce9987ad6b2bc79305634f89d90b90102bcd59a57e7135b8e3ceb93c0597117b");

            var hash = Convert.FromBase64String(HashHelper.HMACSHA1(text, secret));
            BitConverter.ToString(hash).Replace("-", "").ToLower().ShouldBe("b8392c23690ccf871f37ec270be1582dec57a503");
            var hash256 = Convert.FromBase64String(HashHelper.HMACSHA256(text, secret));
            BitConverter.ToString(hash256).Replace("-", "").ToLower()
                .ShouldBe("2f443685592900e619f2f3b2350c3c8a5738e2e7a26bc9a244d3393c3cd6abd6");
        }
    }
}