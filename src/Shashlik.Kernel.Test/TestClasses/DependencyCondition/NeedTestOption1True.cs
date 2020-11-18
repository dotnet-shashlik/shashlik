using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(bool), "TestOptions1.Enable", true, DefaultValue = true)]
    [Singleton]
    public class NeedTestOption1True
    {
        // should be not null
    }
}