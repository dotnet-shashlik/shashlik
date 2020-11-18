using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    // default is ALL
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True))]
    [Singleton]
    public class DependsOnAll
    {
        // should be null
    }
}