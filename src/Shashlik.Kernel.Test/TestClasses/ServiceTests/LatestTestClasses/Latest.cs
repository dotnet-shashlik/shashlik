using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests.LatestTestClasses
{
    [Singleton]
    public interface ILatest
    {
    }

    public class LatestA : ILatest
    {
    }

    [LatestImplementation]
    public class LatestB : ILatest
    {
    }

    public class LatestC : ILatest
    {
    }
}