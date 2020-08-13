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
using Senparc.CO2NET.Extensions;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.Helpers;
using System.Web;
using Guc.Utils.Extensions;
using Guc.Utils;
using Guc.AspNetCore;
using Guc.Response;

namespace Guc.Wx.Controllers
{
    /// <summary>
    /// 微信jssdk
    /// </summary>
    [Route("[controller]")]
    public class WxjssdkController : ControllerBase
    {
        /// <summary>
        /// 获取微信js-sdk配置对象,jsApiList参数前端自己拼
        /// </summary>
        /// <param name="url">前端当前网页的url,不包含#及后面的,且 urlencode后的值</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string url, [FromServices]IWxSettings wxSettings, [FromServices]IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                return NotFound();

            if (string.IsNullOrWhiteSpace(url))
                throw new GucException("url参数错误");

            url = HttpUtility.UrlDecode(url);
            if (!url.IsMatch(Consts.Regexs.Url))
                throw new GucException("url参数错误");

            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(wxSettings.Setting.WeixinAppId, wxSettings.Setting.WeixinAppSecret, url);
            return new ObjectResult(jssdkUiPackage);
        }
    }
}