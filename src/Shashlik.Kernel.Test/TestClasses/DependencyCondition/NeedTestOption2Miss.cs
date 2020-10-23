using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(bool?), "TestOptions2:Enable", true, null)]
    public class NeedTestOption2Miss : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}