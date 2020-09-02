using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty("TestOptions1:Enable", "true")]
    public class NeedTestOption1True : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}