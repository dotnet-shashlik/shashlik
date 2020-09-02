using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    // default is ALL
    [DependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True))]
    public class DependsOnAll : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}