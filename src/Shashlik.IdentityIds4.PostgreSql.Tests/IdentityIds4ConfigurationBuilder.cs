using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.IdentityIds4.PostgreSql.Tests
{
    public class IdentityIds4ConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            builder.AddYamlFile(new FileInfo("./config.yaml").FullName);
            builder.AddYamlFile(new FileInfo("./ids4.yaml").FullName);
        }
    }
}