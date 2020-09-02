using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty("TestOptions2:Enable", "true", matchIfMissing: true)]
    public class NeedTestOption2Miss : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}