using Guc.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET.HttpUtility;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.RegisterServices;
using System;
using System.Net;
using Guc.Kernel;

namespace Guc.Wx
{
    public static class Extensions
    {
        /*
         * 配置参考
         * https://github.com/JeffreySu/WeiXinMPSDK/blob/master/Samples/Senparc.Weixin.MP.Sample.vs2017/Senparc.Weixin.MP.CoreSample/Startup.cs
         * **/

        /// <summary>
        /// 增加guc微信sdk服务,默认配置内存缓存策略
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="wxSdkConfiguration">盛派sdk/SdkOptions配置节点</param>
        /// <param name="wxTemplateMsgConfigurations">格式化微信模板消息配置</param>
        /// <param name="useEmptyTemplateMsg">使用空的微信模板消息,就是不会真正的发送</param>
        /// <returns></returns>
        public static IKernelBuilder AddWx(
            this IKernelBuilder kernelBuilder,
            IConfigurationSection wxSdkConfiguration,
            IConfigurationSection wxTemplateMsgConfigurations = null,
            bool useEmptyTemplateMsg = false)
        {
            var services = kernelBuilder.Services;

            if (useEmptyTemplateMsg)
                services.AddTransient<IWxTemplateMsg, EmptyWxTemplateMsg>();
            else
                services.AddTransient<IWxTemplateMsg, DefaultWxTemplateMsg>();

            services.Configure<WxSdkOptions>(wxSdkConfiguration.GetSection("SdkOptions"));
            if (wxTemplateMsgConfigurations != null)
                services.Configure<WxTemplateMsgOptions>(wxTemplateMsgConfigurations);

            var sdkOptions = wxSdkConfiguration.GetSection("SdkOptions").Get<WxSdkOptions>();
            if (sdkOptions == null)
                throw new Exception("SdkOptions can't be null.");
            if (!sdkOptions.ThisHost.IsMatch(Guc.Utils.Consts.Regexs.Url))
                throw new Exception("SdkOptions.ThisHost must be url.");

            if (!sdkOptions.ProxyHost.IsNullOrWhiteSpace() && sdkOptions.ProxyPort.HasValue)
            {
                if (sdkOptions.ProxyUserName.IsNullOrWhiteSpace() || sdkOptions.ProxyPassword.IsNullOrWhiteSpace())
                    RequestUtility.SenparcHttpClientWebProxy = new System.Net.WebProxy(sdkOptions.ProxyHost, sdkOptions.ProxyPort.Value);
                else
                {
                    NetworkCredential credential = new NetworkCredential(sdkOptions.ProxyUserName, sdkOptions.ProxyPassword);
                    RequestUtility.SenparcHttpClientWebProxy = new WebProxy($"{sdkOptions.ProxyHost}:{sdkOptions.ProxyPort}", true, null, credential);
                }
            }

            services
                    //Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(wxSdkConfiguration);

            return kernelBuilder;
        }


    }
}
