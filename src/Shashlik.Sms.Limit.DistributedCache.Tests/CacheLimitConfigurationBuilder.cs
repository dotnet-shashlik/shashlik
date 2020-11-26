using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.Sms.Limit.DistributedCache.Tests
{
    public class CacheLimitConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            builder.AddYamlFile(new FileInfo("./config.yaml").FullName);
        }
    }
}