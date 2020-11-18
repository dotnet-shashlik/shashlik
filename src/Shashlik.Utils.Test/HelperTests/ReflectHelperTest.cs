using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    [DisplayAttribute]
    public class ReflectHelperTest
    {
        [Fact]
        public void GetReferredAssembliesTest()
        {
            var list = ReflectionHelper.GetReferredAssemblies<ReflectionHelper>();
            list.ShouldNotBeEmpty();
            var listByType = ReflectionHelper.GetReferredAssemblies(typeof(ReflectionHelper));
            listByType.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetFinalSubTest()
        {
            var list = ReflectionHelper.GetFinalSubTypes<ReflectionHelper>();
            list.ShouldNotBeEmpty();
            var listByAssembly = ReflectionHelper.GetFinalSubTypes<ReflectionHelper>(Assembly.GetExecutingAssembly());
            listByAssembly.ShouldBeEmpty();
        }

        [Fact]
        public void GetTypesByAttributesTest()
        {
            var result = ReflectionHelper.GetTypesByAttributes<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = ReflectionHelper.GetTypesByAttributes(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetTypesAndAttributeTest()
        {
            var result = ReflectionHelper.GetTypesAndAttribute<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = ReflectionHelper.GetTypesAndAttribute(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }
    }
}