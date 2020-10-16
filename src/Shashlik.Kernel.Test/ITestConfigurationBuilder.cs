using System.IO;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Test
{
    public interface ITestConfigurationBuilder
    {
        void Build(IConfigurationBuilder builder);
    }

    public class KernelTestConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            var file = new FileInfo("./settings/appsettings.json").FullName;
            builder.AddJsonFile(file);
        }
    }
}