using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests.LatestTestClasses
{
    public interface ILatest
    {
    }

    public class LatestA : ILatest
    {
    }

    [Primary]
    public class LatestB : ILatest
    {
    }

    public class LatestC : ILatest
    {
    }
}