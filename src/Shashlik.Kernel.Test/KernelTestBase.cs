using System;
using System.Collections;
using System.Collections.Generic;
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

        protected IEnumerable<T> GetServices<T>()
        {
            return ServiceScope.ServiceProvider.GetServices<T>();
        }

        public virtual void Dispose()
        {
            ServiceScope.Dispose();
        }
    }
}