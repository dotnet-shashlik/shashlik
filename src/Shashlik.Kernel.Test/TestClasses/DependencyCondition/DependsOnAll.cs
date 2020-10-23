using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    // default is ALL
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True))]
    public class DependsOnAll : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}