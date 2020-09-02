using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [HostEnvironment("Development")]
    public class EnvConditionDev : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}