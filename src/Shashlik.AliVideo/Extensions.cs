using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Guc.Kernel;

namespace Guc.AliVideo
{
    public static class Extensions
    {
        /// <summary>
        /// 阿里云oss
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IKernelBuilder AddAliVideo(this IKernelBuilder kernelBuilder, IConfigurationSection configuration)
        {
            if (configuration == null)
                throw new System.ArgumentNullException(nameof(configuration));

            kernelBuilder.Services.Configure<AliVideoOptions>(configuration);
            return kernelBuilder;
        }
    }
}
