using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.Sms.Cap.Tests
{
    public class SmsCapConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            builder.AddYamlFile(new FileInfo("./config.yaml").FullName);
        }
    }
}