using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(NeedTestOption2Miss))]
    [Singleton]
    public class DependsOnNeedTestOption1True
    {
        // should be not null
    }
}