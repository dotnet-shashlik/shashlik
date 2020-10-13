using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnHostEnvironment("Production")]
    public class EnvConditionProd : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}