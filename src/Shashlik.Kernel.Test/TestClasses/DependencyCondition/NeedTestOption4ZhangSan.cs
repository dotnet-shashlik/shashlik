using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "张三")]
    public class NeedTestOption4ZhangSan1 : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }

    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "ZhangSan", IgnoreCase = false)]
    public class NeedTestOption4ZhangSan2 : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }

    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "zhangsan", IgnoreCase = true)]
    public class NeedTestOption4ZhangSan3 : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}