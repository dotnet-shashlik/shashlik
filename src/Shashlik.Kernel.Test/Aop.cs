using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test
{
    public interface ICustomService
    {
        [CustomInterceptor]
        void Call1();

        [CustomInterceptor2]
        void Call2();

        void Call3();
    }

    public class CustomService : ICustomService, ISingleton
    {
        public void Call1()
        {
        }

        public void Call2()
        {
        }

        public void Call3()
        {
            
        }
    }

    public class CustomInterceptorAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }
    }
    
    public class CustomInterceptor2Attribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!");
                throw;
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }
    }
}