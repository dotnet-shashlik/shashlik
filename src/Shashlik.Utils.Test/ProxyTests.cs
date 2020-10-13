using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class ProxyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ProxyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void test()
        {
            //创建拦截器对象   
            SampleInterceptor interceptor = new SampleInterceptor(_testOutputHelper.WriteLine);
            SampleInterceptor2 interceptor2 = new SampleInterceptor2(_testOutputHelper.WriteLine);
            //给person类生成代理   
            ProxyGenerator generator = new ProxyGenerator();
            IPerson p = generator.CreateInterfaceProxyWithTarget<IPerson>(new Person(), interceptor);
            
            
            //执行方法看效果   
            p.Doing();
        }
    }

    /// <summary>   
    ///IPerson 的摘要说明   
    /// </summary>   
    public interface IPerson
    {
        /// <summary>   
        /// 姓名   
        /// </summary>   
        string Name { get; }

        /// <summary>   
        /// 地址   
        /// </summary>   
        string Address { get; }

        /// <summary>   
        /// 正在做什么   
        /// </summary>   
        /// <returns></returns>   
        Task Doing();
    }

    /// <summary>   
    ///Person 的摘要说明   
    /// </summary>   
    public class Person : IPerson
    {
        public Person()
        {
            //   
            //TODO: 在此处添加构造函数逻辑   
            //   
        }

        #region IPerson 成员

        public string Name
        {
            get { return "我是花生米"; }
        }

        public string Address
        {
            get { return "我住在 http://pignut-wang.iteye.com/ "; }
        }

        public async Task Doing()
        {
            await Task.Delay(1000);
        }

        #endregion
    }


    /// <summary>  
    /// 拦截器示例  
    /// </summary>  
    public class SampleInterceptor : IInterceptor
    {
        private Action<string> output { get; set; }

        public SampleInterceptor(Action<string> output)
        {
            this.output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            output("开始进入拦截器");

            MethodInfo concreteMethod = invocation.GetConcreteMethod();


            if (!invocation.MethodInvocationTarget.IsAbstract)
            {
                output("开始执行 " + invocation.InvocationTarget.GetType());
                output("开始执行 " + concreteMethod.Name);

                //执行原对象中的方法  
                invocation.Proceed();

                output("执行结果 " + invocation.ReturnValue);
            }

            output("执行完毕");
        }
    }


    /// <summary>  
    /// 拦截器示例  
    /// </summary>  
    public class SampleInterceptor2 : IInterceptor
    {
        private Action<string> output { get; set; }

        public SampleInterceptor2(Action<string> output)
        {
            this.output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            output("2开始进入拦截器");

            MethodInfo concreteMethod = invocation.GetConcreteMethod();


            if (!invocation.MethodInvocationTarget.IsAbstract)
            {
                output("2开始执行 " + invocation.InvocationTarget.GetType());
                output("2开始执行 " + concreteMethod.Name);

                //执行原对象中的方法  
                invocation.Proceed();

                output("2执行结果 " + invocation.ReturnValue);
            }

            output("2执行完毕");
        }
    }
}