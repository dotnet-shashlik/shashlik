using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(bool), "TestOptions2:Enable", true, DefaultValue = true)]
    [Singleton]
    public class NeedTestOption2Miss
    {
        // should be not null
    }
}