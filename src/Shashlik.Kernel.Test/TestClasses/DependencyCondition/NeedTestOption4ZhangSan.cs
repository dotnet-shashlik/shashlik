using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "张三")]
    [Singleton]
    public class NeedTestOption4ZhangSan1
    {
        // should be null
    }

    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "ZhangSan", IgnoreCase = false)]
    [Singleton]
    public class NeedTestOption4ZhangSan2
    {
        // should be null
    }

    [ConditionOnProperty(typeof(string), "TestOptions4:Name", "zhangsan", IgnoreCase = true)]
    [Singleton]
    public class NeedTestOption4ZhangSan3
    {
        // should be not null
    }
}