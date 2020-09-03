using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    [Display(Name = "AssemblyHelper Test")]
    public class AssemblyHelperTest
    {
        [Fact]
        public void GetReferredAssembliesTest()
        {
            var list = AssemblyHelper.GetReferredAssemblies<AssemblyHelper>();
            list.ShouldNotBeEmpty();
            var listByType = AssemblyHelper.GetReferredAssemblies(typeof(AssemblyHelper));
            listByType.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetFinalSubTest()
        {
            var list = AssemblyHelper.GetFinalSubTypes<AssemblyHelper>();
            list.ShouldNotBeEmpty();
            var listByAssembly = AssemblyHelper.GetFinalSubTypes<AssemblyHelper>(Assembly.GetExecutingAssembly());
            listByAssembly.ShouldBeEmpty();
        }

        [Fact]
        public void GetTypesByAttributesTest()
        {
            var result = AssemblyHelper.GetTypesByAttributes<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = AssemblyHelper.GetTypesByAttributes(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetTypesAndAttributeTest()
        {
            var result = AssemblyHelper.GetTypesAndAttribute<DisplayAttribute>();
            result.ShouldNotBeEmpty();
            var result2 = AssemblyHelper.GetTypesAndAttribute(typeof(DisplayAttribute));
            result2.ShouldNotBeEmpty();
        }
    }
}