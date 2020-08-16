using System;
using System.Threading.Tasks;
using Shashlik.Kernel;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin.MP.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.AspNetCore;
using Microsoft.Extensions.Logging;
using Senparc.Weixin;
using Microsoft.AspNetCore.Hosting;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.NeuChar.Entities;
using Shashlik.Utils.Common;
using System.Threading;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.CO2NET.RegisterServices;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Containers;
using Senparc.CO2NET.AspNet;
using Shashlik.Senparc.CsRedis;
using Senparc.Weixin.WxOpen;
using Senparc.Weixin.TenPay;
using Microsoft.Extensions.Hosting;

namespace Shashlik.Wx
{
    public static class Extensions
    {
        static WxApiOptions WxApiOptions { get; set; }

        /// <summary>
        /// 使用微信api
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig UseWxApi(this IKernelAspNetCoreConfig config, Action<WxApiOptions> optionsAction = null)
        {
            WxApiOptions options = new WxApiOptions();
            optionsAction?.Invoke(options);
            WxApiOptions = options;

            if (!options.WxJsSdk.IsNullOrWhiteSpace()) config.App.Map(options.WxJsSdk, r => r.Run(WxJsSdk));
            //if (!options.WxPayNotify.IsNullOrWhiteSpace()) config.App.Map(options.WxPayNotify, r => r.Run(WxPayNotify));
            //if (!options.WxOauthSnsapiBaseCallback.IsNullOrWhiteSpace()) config.App.Map(options.WxOauthSnsapiBaseCallback, r => r.Run(WxOauthSnsapiBaseCallback));
            //if (!options.WxOauthSnsapiUserinfoCallback.IsNullOrWhiteSpace()) config.App.Map(options.WxOauthSnsapiUserinfoCallback, r => r.Run(WxOauthSnsapiUserinfoCallback));
            //if (!options.WxOauthSnsapiBase.IsNullOrWhiteSpace()) config.App.Map(options.WxOauthSnsapiBase, r => r.Run(WxOauthSnsapiBase));
            //if (!options.WxOauthSnsapiUserinfo.IsNullOrWhiteSpace()) config.App.Map(options.WxOauthSnsapiUserinfo, r => r.Run(WxOauthSnsapiUserinfo));
            if (!options.WxServerResponse.IsNullOrWhiteSpace()) config.App.Map(options.WxServerResponse, r => r.Run(WxServerResponse));

            return config;
        }

        /// <summary>
        /// 使用微信服务,默认注册微信公众号和微信支付
        /// </summary>
        /// <param name="kernelConfig"></param>
        /// <param name="appName">应用名称</param>
        /// <param name="isDebug">是否调式模式(debug模式可以输出日志)</param>
        /// <param name="customRegister">自定义的其他微信配置,参考盛派sdk文档</param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig UseWx(this IKernelAspNetCoreConfig kernelConfig, string appName, Action<IRegisterService> customRegister = null)
        {
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentException("appName can't be null.", nameof(appName));

            IOptions<SenparcSetting> senparcSetting = kernelConfig.ServiceProvider.GetRequiredService<IOptions<SenparcSetting>>();
            IOptions<SenparcWeixinSetting> senparcWeixinSetting = kernelConfig.ServiceProvider.GetRequiredService<IOptions<SenparcWeixinSetting>>();
            IHostEnvironment env = kernelConfig.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var csRedisClient = kernelConfig.ServiceProvider.GetRequiredService<CSRedis.CSRedisClient>();
            ILogger logger = kernelConfig.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("wxconfig");


            var register = kernelConfig.App.UseSenparcGlobal(env, senparcSetting.Value, globalRegister =>
                {
                    globalRegister.ChangeDefaultCacheNamespace("HEALTH_WX_CACHE");
                    // 注册CRedisCore缓存
                    Shashlik.Senparc.CsRedis.Register.UseKeyValueRedisNow(csRedisClient);
                })
                 // 加载微信配置
                 .UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value)
                 // 注册公众号（可注册多个）
                 .RegisterMpAccount(senparcWeixinSetting.Value, appName)
                 // 注册小程序 可注册多个
                 .RegisterWxOpenAccount(senparcWeixinSetting.Value, appName)
                 // 注册最新微信支付版本（V3）（可注册多个）
                 // 记录到同一个 SenparcWeixinSettingItem 对象中
                 .RegisterTenpayV3(senparcWeixinSetting.Value, appName)
                 ;

            // 自定义配置
            customRegister?.Invoke(register);

            // 注册access_token管理容器
            AccessTokenContainer.RegisterAsync(senparcWeixinSetting.Value.WeixinAppId, senparcWeixinSetting.Value.WeixinAppSecret).GetAwaiter().GetResult();
            return kernelConfig;
        }

        /// <summary>
        /// 获取jssdk配置对象
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        static async Task WxJsSdk(HttpContext httpContext)
        {
            httpContext.Request.Query.TryGetValue("url", out var _url);
            string url = _url.ToString();
            if (string.IsNullOrWhiteSpace(url))
            {
                await httpContext.WriteText("url参数错误");
                return;
            }

            url = url.UrlDecode();
            if (!url.IsMatch(Consts.Regexs.Url))
            {
                await httpContext.WriteText("url参数错误");
                return;
            }

            var wxSettings = httpContext.RequestServices.GetService<IWxSettings>();
            var jsSdkUiPackage = await JSSDKHelper.GetJsSdkUiPackageAsync(wxSettings.Setting.WeixinAppId, wxSettings.Setting.WeixinAppSecret, url);

            // 如果是基于本地缓存 ,就需要每次就去获取新的token,就用下面的代码,分布式缓存就不需要了
            //var ticket = CommonApi.GetTicketByAccessToken(wxSettings.Token);
            ////获取时间戳
            //var timestamp = JSSDKHelper.GetTimestamp();
            ////获取随机码
            //string nonceStr = JSSDKHelper.GetNoncestr();
            //var signature = JSSDKHelper.GetSignature(ticket.ticket, nonceStr, timestamp, url);
            //var jssdkUiPackage = new JsSdkUiPackage(wxSettings.Setting.WeixinAppId, timestamp, nonceStr, signature);

            // 不直接调用,因为access_token 的问题
            await httpContext.WriteJson(new ResponseResult(jsSdkUiPackage));
        }

        static async Task WriteText(this HttpContext httpContext, string content, string contentType = "text/plain; charset=utf-8")
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(content);
        }

        static async Task WriteXml(this HttpContext httpContext, string content, string contentType = "text/xml; charset=utf-8")
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(content);
        }

        static async Task WriteJson<T>(this HttpContext httpContext, T result, string contentType = "application/json; charset=utf-8")
            where T : class
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(result.ToJsonWithCamelCasePropertyNames());
        }

        static async Task WxServerResponse(HttpContext httpContext)
        {
            if (httpContext.Request.Method.EqualsIgnoreCase("get"))
            {
                // get请求用于验证
                httpContext.Request.Query.TryGetValue("echostr", out var echoStr);
                await WriteText(httpContext, echoStr);
                return;
            }
            else
            {
                try
                {
                    httpContext.Request.Query.TryGetValue("signature", out var signature);
                    httpContext.Request.Query.TryGetValue("timestamp", out var timestamp);
                    httpContext.Request.Query.TryGetValue("nonce", out var nonce);
                    httpContext.Request.Query.TryGetValue("msg_signature", out var msgSignature);
                    var wxSettings = httpContext.RequestServices.GetService<IWxSettings>();
                    PostModel postModel = new PostModel
                    {
                        Signature = signature,
                        Timestamp = timestamp,
                        Nonce = nonce,
                        Token = wxSettings.Setting.Token,
                        EncodingAESKey = wxSettings.Setting.EncodingAESKey,
                        AppId = wxSettings.Setting.WeixinAppId,
                        Msg_Signature = msgSignature
                    };

                    if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, postModel.Token))
                    {
                        await WriteText(httpContext, "success");
                        return;
                    }

                    var messageHandler = new WxMessageHandler(httpContext, httpContext.Request.GetRequestMemoryStream(), postModel, 10);
                    await messageHandler.ExecuteAsync(CancellationToken.None);

                    if (messageHandler.ResponseMessage == null
                        || messageHandler.FinalResponseDocument == null
                        || messageHandler.ResponseMessage is ResponseMessageNoResponse)
                    {
                        await WriteText(httpContext, "success");
                        return;
                    }

                    // 如果请求需要转发
                    if (messageHandler.ResponseMessage is ResponseRedirect)
                    {
                        var url = $"{(messageHandler.ResponseMessage as ResponseRedirect).Host.TrimEnd('/')}{WxApiOptions.WxServerResponse}" + httpContext.Request.QueryString.Value;
                        // 这里没有form参数
                        var body = httpContext.Request.Body.ReadToString();
                        var res = await HttpHelper.Post(url, body, httpContext.Request.ContentType);
                        await WriteText(httpContext, res.Content);
                        return;
                    }

                    // 输出xml响应
                    await WriteText(httpContext, messageHandler.FinalResponseDocument.ToString());
                }
                catch (Exception ex)
                {
                    var logger = httpContext.RequestServices.GetService<ILoggerFactory>().CreateLogger(nameof(WxServerResponse));
                    logger.LogError(ex, "微信服务器响应错误");
                    await WriteText(httpContext, "success");
                }
            }
        }
    }
}
