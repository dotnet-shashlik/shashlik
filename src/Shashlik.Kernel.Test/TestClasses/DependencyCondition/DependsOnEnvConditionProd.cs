using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd))]
    [Singleton]
    public class DependsOnEnvConditionProd
    {
        // should be null
    }
}