using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin.TenPay.V3;
using Microsoft.Extensions.Configuration;
using Senparc.Weixin;
using Microsoft.AspNetCore.Hosting;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Guc.Wx.Notifies;
using Microsoft.Extensions.DependencyInjection;
using Guc.AspNetCore;
using Microsoft.Extensions.Options;
using Guc.Utils.Extensions;

namespace Guc.Wx.Controllers
{
    /// <summary>
    /// 微信授权登录
    /// </summary>
    [Route("[controller]")]
    [AllowAnonymous]
    [NoWrapper]
    public class WxOAuthController : ControllerBase
    {
        IWxSettings WxSettings { get; }
        IOptionsSnapshot<WxsdkOptions> Options { get; }
        ILogger<WxOAuthController> Logger { get; }
        public WxOAuthController(
            IWxSettings wxData,
            ILogger<WxOAuthController> logger,
            IOptionsSnapshot<WxsdkOptions> options
            )
        {
            Logger = logger;
            WxSettings = wxData;
            Options = options;
        }

        /// <summary>
        /// oauth 登录,获取用户id(不弹出授权页面，直接跳转，只能获取用户openid)非ajax请求,使用页面跳转
        /// </summary>
        /// <param name="returnUrl">最后回到哪个页面上(urlencode)</param>
        /// <param name="type">授权用途的类型:login(微信登录),bind(用户微信绑定)</param>
        /// <param name="attach">附加数据(urlencode),可以为空</param>
        /// <returns></returns>
        [HttpGet("snsapi_base")]
        public IActionResult snsapi_base(string returnUrl, string type, string attach)
        {
            if (returnUrl.IsNullOrWhiteSpace())
                return Content("回调地址参数错误");
            var validReturnUrl = returnUrl.Trim().UrlDecode();

            if (!Guc.Utils.Extensions.StringExtensions.IsUrl(validReturnUrl))
                return Content("回调地址参数错误");

            // 当前api主域
            string host = Options.Value.ThisHost.TrimEnd('/');
            string callback = $"{host}/wxoauth/snsapi_base/callback";
            callback = Guc.Utils.Extensions.StringExtensions.UrlArgsCombine(callback, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("returnUrl",returnUrl),
                new KeyValuePair<string, object>("type",type),
                new KeyValuePair<string, object>("attach",attach),
            });

            // 生成微信oauth授权登录地址
            var url = OAuthApi.GetAuthorizeUrl(WxSettings.Setting.WeixinAppId, callback, "guc_wxsdk_state", OAuthScope.snsapi_base);

            // 跳转至微信授权
            return Redirect(url);
        }

        /// <summary>
        /// oauth 登录,获取用户id(弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息)非ajax请求,使用页面跳转
        /// </summary>
        /// <param name="returnUrl">最后回到哪个页面上,urlencode后</param>
        /// <param name="type">授权用途的类型:login(微信登录),bind(用户微信绑定)</param>
        /// <param name="attach">附加数据(urlencode),可以为空</param>
        /// <returns></returns>
        [HttpGet("snsapi_userinfo")]
        public IActionResult snsapi_userinfo(string returnUrl, string type, string attach)
        {
            if (returnUrl.IsNullOrWhiteSpace())
                return Content("回调地址参数错误");
            var validReturnUrl = returnUrl.Trim().UrlDecode();
            if (!Guc.Utils.Extensions.StringExtensions.IsUrl(validReturnUrl))
                return Content("回调地址参数错误");

            // 当前api主域
            string host = Options.Value.ThisHost.TrimEnd('/');
            string callback = $"{host}/wxoauth/snsapi_userinfo/callback";
            callback = Guc.Utils.Extensions.StringExtensions.UrlArgsCombine(callback, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("returnUrl",returnUrl),
                new KeyValuePair<string, object>("type",type),
                new KeyValuePair<string, object>("attach",attach),
            });

            // 生成微信oauth授权登录地址
            var url = OAuthApi.GetAuthorizeUrl(WxSettings.Setting.WeixinAppId, callback, "guc_wxsdk_state", OAuthScope.snsapi_userinfo);

            // 跳转至微信授权
            return Redirect(url);
        }

        /// <summary>
        /// 微信oauth登录回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("snsapi_base/callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> snsapi_baseOAuthCallback(string code, string state, string returnUrl, string type, string attach)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return Content("您拒绝了微信授权！");

                if (state != "guc_wxsdk_state")
                    return Content("微信授权验证失败");

                OAuthAccessTokenResult openIdResult = null;
                // 附加到前端returnUrl的参数
                List<KeyValuePair<string, object>> extendArgs = new List<KeyValuePair<string, object>>();
                try
                {
                    //通过，用code换取access_token
                    openIdResult = await OAuthApi.GetAccessTokenAsync(WxSettings.Setting.WeixinAppId, WxSettings.Setting.WeixinAppSecret, code);
                    if (openIdResult.errcode != ReturnCode.请求成功)
                        return Content("微信授权错误：" + openIdResult.errmsg);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "snsapi_base:获取微信access token错误");
                    return Content(ex.Message);
                }

                // 回调处理
                var callbacks = HttpContext.RequestServices.GetServices<ISnsapiBaseOAuthNotify>();
                if (callbacks != null && callbacks.Any())
                    foreach (var item in callbacks
                        .WhereIf(!type.IsNullOrWhiteSpace(), r => r.Type.EqualsIgnoreCase(type))
                        .WhereIf(type.IsNullOrWhiteSpace(), r => r.Type.IsNullOrWhiteSpace())
                        .OrderBy(r => r.Priority))
                    {
                        item.Handle(openIdResult.openid, type, attach, out var urlArgs);
                        if (urlArgs != null && urlArgs.Any())
                            extendArgs.AddRange(urlArgs);
                    }

                extendArgs.AddRange(new List<KeyValuePair<string, object>> {
                    new KeyValuePair<string, object>("openid",openIdResult.openid),
                    new KeyValuePair<string, object>("type",type),
                    new KeyValuePair<string, object>("attach",attach),
                });

                returnUrl = returnUrl.UrlDecode();
                returnUrl = Guc.Utils.Extensions.StringExtensions.UrlArgsCombine(returnUrl, extendArgs);
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"微信登录授权回调错误");
                return Content("微信登录授权错误");
            }
        }

        /// <summary>
        /// 微信oauth登录回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("snsapi_userinfo/callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> snsapi_userinfoOAuthCallback(string code, string state, string returnUrl, string type, string attach)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return Content("您拒绝了微信授权！");

                if (state != "guc_wxsdk_state")
                    return Content("微信授权验证失败");

                OAuthAccessTokenResult openIdResult = null;
                //通过，用code换取access_token
                try
                {
                    openIdResult = await OAuthApi.GetAccessTokenAsync(WxSettings.Setting.WeixinAppId, WxSettings.Setting.WeixinAppSecret, code);
                    if (openIdResult.errcode != ReturnCode.请求成功)
                        return Content("错误：" + openIdResult.errmsg);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "snsapi_userinfo:获取微信access token错误");
                    return Content(ex.Message);
                }

                // 附加到前端returnUrl的参数
                List<KeyValuePair<string, object>> extendArgs = new List<KeyValuePair<string, object>>();
                // 因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                OAuthUserInfo userInfo = null;
                try
                {
                    userInfo = await OAuthApi.GetUserInfoAsync(openIdResult.access_token, openIdResult.openid);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "snsapi_userinfo:获取微信用户数据错误");
                    return Content(ex.Message);
                }

                // 回调处理
                var callbacks = HttpContext.RequestServices.GetServices<ISnsapiUserInfoOAuthNotify>();

                callbacks =
                callbacks
                        .WhereIf(!type.IsNullOrWhiteSpace(), r => r.Type.EqualsIgnoreCase(type))
                        .WhereIf(type.IsNullOrWhiteSpace(), r => r.Type.IsNullOrWhiteSpace())
                        .OrderBy(r => r.Priority)
                        .ToList();

                if (callbacks != null && callbacks.Any())
                    foreach (var item in callbacks)
                    {
                        item.Handle(userInfo, type, attach, out var urlArgs);
                        if (urlArgs != null && urlArgs.Any())
                            extendArgs.AddRange(urlArgs);
                    }

                extendArgs.AddRange(new List<KeyValuePair<string, object>> {
                    new KeyValuePair<string, object>("openid",userInfo.openid),
                    new KeyValuePair<string, object>("type",type),
                    new KeyValuePair<string, object>("attach",attach),
                });

                returnUrl = returnUrl.UrlDecode();
                returnUrl = Guc.Utils.Extensions.StringExtensions.UrlArgsCombine(returnUrl, extendArgs);

                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"微信登录授权回调错误");
                return Content("微信登录授权错误");
            }
        }
    }
}