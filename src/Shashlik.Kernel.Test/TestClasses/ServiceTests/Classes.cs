using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests
{
    public interface IA
    {
    }

    [Transient()]
    public class A1 : IA
    {
    }

    [Transient(), Primary]
    public class A2 : IA
    {
    }

    [Transient()]
    public class A3 : IA
    {
    }

    public interface IB<T>
    {
    }

    [Transient(), Primary]
    public class B1 : IB<int>
    {
    }

    [Transient()]
    public class B2<T> : IB<T>
    {
    }

    public interface IC
    {
    }

    [Transient]
    public class C<T> : IC
    {
    }
}