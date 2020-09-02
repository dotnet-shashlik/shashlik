using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [HostEnvironment("Production")]
    public class EnvConditionProd : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}