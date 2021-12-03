using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.Sms.Limit.Redis.Tests
{
    public class RedisLimitConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            builder.AddYamlFile(new FileInfo("./config.yaml").FullName);
        }
    }
}