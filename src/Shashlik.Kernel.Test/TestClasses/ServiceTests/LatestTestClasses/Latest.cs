using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests.LatestTestClasses
{
    [Singleton]
    public interface ILatest
    {
    }

    public class LatestA
    {
    }

    [LatestImplementation]
    public class LatestB
    {
    }

    public class LatestC
    {
    }
}