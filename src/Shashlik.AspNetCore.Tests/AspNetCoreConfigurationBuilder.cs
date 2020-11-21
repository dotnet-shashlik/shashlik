using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.AspNetCore.Tests
{
    public class AspNetCoreConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            builder.AddYamlFile(new FileInfo("./config.yaml").FullName);
        }
    }
}