using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(bool?), "TestOptions1:Enable", true, null)]
    public class NeedTestOption1True : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}