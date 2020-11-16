using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.DataProtection.Redis.Tests
{
    public class PostgreSqlDataProtectionTestConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            var file = new FileInfo("./config.json").FullName;
            builder.AddJsonFile(file);
        }
    }
}