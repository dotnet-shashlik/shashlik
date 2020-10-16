using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Shashlik.Kernel.Test
{
    public abstract class KernelTestBase : IClassFixture<TestWebApplicationFactory<TestStartup>>, IDisposable
    {
        protected TestWebApplicationFactory<TestStartup> Factory { get; }
        protected HttpClient HttpClient { get; }
        protected IServiceScope ServiceScope { get; }


        public KernelTestBase(TestWebApplicationFactory<TestStartup> factory)
        {
            Factory = factory;
            HttpClient = factory.CreateClient();
            ServiceScope = factory.Services.CreateScope();
        }

        protected T GetService<T>()
        {
            return ServiceScope.ServiceProvider.GetService<T>();
        }

        public virtual void Dispose()
        {
            ServiceScope.Dispose();
        }
    }
}
