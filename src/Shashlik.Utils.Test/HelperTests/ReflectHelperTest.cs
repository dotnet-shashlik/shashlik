using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    public class ReflectHelperTest
    {
        [Fact]
        public void GetReferredAssembliesTest()
        {
            var list = ReflectHelper.GetReferredAssemblies<ReflectHelper>();
            list.ShouldNotBeEmpty();
            var listByType = ReflectHelper.GetReferredAssemblies(typeof(ReflectHelper));
            listByType.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetFinalSubTest()
        {
            var list = ReflectHelper.GetFinalSubTypes<ReflectHelper>();
            list.ShouldNotBeEmpty();
            var listByAssembly = ReflectHelper.GetFinalSubTypes<ReflectHelper>(Assembly.GetExecutingAssembly());
            listByAssembly.ShouldBeEmpty();
        }

        [Fact]
        public void GetTypesByAttributesTest()
        {
            var result = ReflectHelper.GetTypesByAttributes<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = ReflectHelper.GetTypesByAttributes(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetTypesAndAttributeTest()
        {
            var result = ReflectHelper.GetTypesAndAttribute<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = ReflectHelper.GetTypesAndAttribute(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }
    }
}