using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.AliyunOss
{
    public static class Extensions
    {
        /// <summary>
        /// 阿里云oss
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IKernelService AddAliyunOss(this IKernelService kernelBuilder, IConfigurationSection configuration)
        {
            if (configuration == null)
                throw new System.ArgumentNullException(nameof(configuration));

            kernelBuilder.Services.Configure<AliyunOssOptions>(configuration);
            return kernelBuilder;
        }
    }
}
