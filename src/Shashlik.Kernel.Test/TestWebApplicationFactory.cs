using System;
using System.IO;
using System.Reflection;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Test
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return
                Host.CreateDefaultBuilder()
                    .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory())
                    .UseEnvironment("Development")
                    .ConfigureAppConfiguration((host, builder) =>
                    {
                        var list = AssemblyHelper.GetFinalSubTypes<ITestConfigurationBuilder>();

                        foreach (var typeInfo in list)
                        {
                            (Activator.CreateInstance(typeInfo) as ITestConfigurationBuilder)
                                !.Build(builder);
                        }

                        builder.AddEnvironmentVariables();
                    })
                    .ConfigureWebHostDefaults(x => { x.UseStartup<TStartup>().UseTestServer(); });
        }
    }
}