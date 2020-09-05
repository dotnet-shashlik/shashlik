using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    public class IdCardHelperTest
    {
        [Fact]
        public void CheckTest()
        {
            string.Empty.IsIdCard().ShouldBe(false);
            "abc".IsIdCard().ShouldBe(false);
            "341321199808080583".IsIdCard().ShouldBe(true);
            "31010720020101453x".IsIdCard().ShouldBe(true);
            "98010720020101455x".IsIdCard().ShouldBe(false);
            "310107200201014531".IsIdCard().ShouldBe(false);
            "34132119982808058b".IsIdCard().ShouldBe(false);
            "34132119982808011x".IsIdCard().ShouldBe(false);
            "3413211a982ed8011x".IsIdCard().ShouldBe(false);
            "110000197913098872".IsIdCard().ShouldBe(false);
        }

        [Fact]
        public void GetIdCardModelTest()
        {
            var model = IdCardHelper.GetIdCardModel("230606198812310574");
            model.ShouldNotBeNull();
            model.Sex.ShouldBe((sbyte)1);
            model = IdCardHelper.GetIdCardModel("341321199808080383");
            model.ShouldBeNull();
        }
    }
}