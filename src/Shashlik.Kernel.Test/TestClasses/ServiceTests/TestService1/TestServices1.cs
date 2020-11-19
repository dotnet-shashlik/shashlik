
using System;
using System.Collections;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService1
{
    [Singleton]
    public interface IA1<T> : IDisposable
    {
    }

    public abstract class B1<T> : IA1<T>, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }

    public class C1<T> : B1<T>
    {
    }


    public class D1<T> : C1<T>
    {
    }

    //############################


    public interface IA2<T> : IDisposable
    {
    }

    [Singleton()]
    public abstract class B2<T> : IA2<T>, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }

    public class C2<T> : B2<T>
    {
    }


    public class D2<T> : C2<T>
    {
    }

    //############################


    public interface IA3<T> : IDisposable
    {
    }


    public abstract class B3<T> : IA3<T>, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }

    [Singleton()]
    public class C3<T> : B3<T>
    {
    }


    public class D3<T> : C3<T>
    {
    }

    //############################


    public interface IA4<T> : IDisposable
    {
    }


    public abstract class B4<T> : IA4<T>, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }


    public class C4<T> : B4<T>
    {
    }

    [Singleton()]
    public class D4<T> : C4<T>
    {
    }

    //############################


    public interface IA5<T> : IDisposable
    {
    }

    [Singleton(typeof(IA5<>), typeof(IComparer), typeof(IDisposable), RequireRegistryInheritedChain = true)]
    public abstract class B5<T> : IA5<T>, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }


    public class C5<T> : B5<T>
    {
    }

    [Singleton()]
    public class D5<T> : C5<T>
    {
    }
}