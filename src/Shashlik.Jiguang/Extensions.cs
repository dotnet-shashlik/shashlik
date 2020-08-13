using Guc.Kernel;
using System;
using System.Collections.Generic;
using System.Text;
using Jiguang.JPush;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guc.PushNotification
{
    public static class Extensions
    {
        /// <summary>
        /// 添加APP消息推送模板
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">APP消息推送模板配置节点</param>
        public static IKernelBuilder AddPushNotificationTemplate(this IKernelBuilder kernelBuilder, IConfiguration configuration)
        {
            var services = kernelBuilder.Services;
            services.Configure<PushNotificationTemplateOptions>(configuration);

            return kernelBuilder;
        }

        /// <summary>
        /// 添加极光推送配置
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">极光推送配置节点</param>
        public static IKernelBuilder AddJiGuangPush(this IKernelBuilder kernelBuilder, IConfiguration configuration)
        {
            var services = kernelBuilder.Services;
            //services.Configure<JiGuangPushOptions>(configuration);

            services.AddSingleton(new JPushClient(configuration.GetValue<string>("Username"),
                configuration.GetValue<string>("Password")));
            return kernelBuilder;
        }
    }
}
