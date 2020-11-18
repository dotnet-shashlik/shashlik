#nullable enable
using System;
using System.Collections;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService2
{
    [Singleton]
    public interface IA1 : IDisposable
    {
    }

    public abstract class B1 : IA1, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }

    public class C1 : B1
    {
    }


    public class D1 : C1
    {
    }

    //############################


    public interface IA2 : IDisposable
    {
    }

    [Singleton()]
    public abstract class B2 : IA2, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }

    public class C2 : B2
    {
    }


    public class D2 : C2
    {
    }

    //############################


    public interface IA3 : IDisposable
    {
    }


    public abstract class B3 : IA3, IComparer
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
    public class C3 : B3
    {
    }


    public class D3 : C3
    {
    }

    //############################


    public interface IA4 : IDisposable
    {
    }


    public abstract class B4 : IA4, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }


    public class C4 : B4
    {
    }

    [Singleton()]
    public class D4 : C4
    {
    }

    //############################


    public interface IA5 : IDisposable
    {
    }

    [Singleton(typeof(IA5), typeof(IComparer), typeof(IDisposable), RequireRegistryInheritedChain = true)]
    public abstract class B5 : IA5, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }


    public class C5 : B5
    {
    }

    [Singleton()]
    public class D5 : C5
    {
    }

    //############################


    public interface IA6 : IDisposable
    {
    }

    [Singleton(typeof(IComparer), typeof(IDisposable), typeof(ICloneable), RequireRegistryInheritedChain = true)]
    public interface IA61<T> : IDisposable, ICloneable
    {
    }

    [Singleton(typeof(IA6), typeof(IComparer), typeof(IDisposable), RequireRegistryInheritedChain = true)]
    public abstract class B6 : IA6, IComparer
    {
        public void Dispose()
        {
        }

        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }


    public class C6 : B6, IA61<int>
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class C61<T> : B6, IA61<T>
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    [Singleton()]
    public class D6 : C6
    {
    }
}