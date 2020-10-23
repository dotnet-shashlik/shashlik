using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd))]
    public class DependsOnEnvConditionProd : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}