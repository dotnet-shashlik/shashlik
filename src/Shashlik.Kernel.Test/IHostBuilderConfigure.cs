using Microsoft.Extensions.Hosting;

namespace Shashlik.Kernel.Test
{
    public interface IHostBuilderConfigure
    {
        public void Configure(IHostBuilder builder);
    }
}