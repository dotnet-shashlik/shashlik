using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shashlik.Utils.Common;
using Shouldly;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Test
{
    public class PinYinExtensionsTests
    {
        [Fact]
        void test1()
        {

            string str = "**你好啊123_(&";
            {

                str.GetPinYin().ShouldBe("**NIHAOA123_(&"); ;
                str.GetPinYinFirst().ShouldBe("**NHA123_(&");
                str.GetPinYinWithOnlyLetterAndNumbers().ShouldBe("NIHAOA123");
                str.GetPinYinFirstWithOnlyLetterAndNumbers().ShouldBe("NHA123");
            }

            // 㵘㙓 算不出来
            str = "魃魈魁鬾魑魅魍魉又双叒叕火炎焱燚水沝淼㵘㙓茕茕孑立沆瀣一气囹圄蘡薁觊觎龃龉";
            str.GetPinYin().ShouldNotBeNullOrWhiteSpace();
            str.GetPinYinFirst().ShouldNotBeNullOrWhiteSpace();
            str.GetPinYinFirstWithOnlyLetterAndNumbers().ShouldNotBeNullOrWhiteSpace();
            str.GetPinYinWithOnlyLetterAndNumbers().ShouldNotBeNullOrWhiteSpace();
        }
    }
}
