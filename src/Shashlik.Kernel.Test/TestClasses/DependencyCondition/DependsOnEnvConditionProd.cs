using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [DependsOn(typeof(EnvConditionProd))]
    public class DependsOnEnvConditionProd : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}