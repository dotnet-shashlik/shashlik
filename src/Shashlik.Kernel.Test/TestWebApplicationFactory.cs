using System.IO;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
                    .ConfigureAppConfiguration((host, config) =>
                    {
                        config
                            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "settings"))
                            // .AddJsonFile("TestOption1.json")
                            // .AddJsonFile("TestOption2.json")
                            .AddEnvironmentVariables();
                    })
                    
                    .ConfigureWebHostDefaults(x =>
                    {
                        x.UseStartup<TStartup>().UseTestServer();
                    });
        }
    }
}
