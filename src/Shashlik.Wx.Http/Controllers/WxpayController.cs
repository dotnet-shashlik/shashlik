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
using Guc.Wx;
using Guc.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Guc.Wx.Notifies;
using Microsoft.Extensions.Options;

namespace Guc.Wx.Controllers
{
    /// <summary>
    /// 微信支付
    /// </summary>
    [Route("[controller]")]
    [NoWrapper]
    public class WxpayController : ControllerBase
    {
        ILogger<WxpayController> logger { get; }
        public WxpayController(ILogger<WxpayController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("notify")]
        public async Task<IActionResult> PayCallback([FromServices]IWxSettings wxSettings, [FromServices]IOptionsSnapshot<WxsdkOptions> options)
        {
            var xml = Request.Body.ReadToString();

            // 获取微信回调数据
            var resHandler = new ResponseHandler(HttpContext);
            logger.LogInformation($"微信支付回调:" + xml);
            // 转换为对象
            var result = new OrderQueryResult(xml);
            // 订单正确和错误的返回给微信的结果数据
            string errStr = @"
<xml>
    <return_code><![CDATA[FAIL]]></return_code>
    <return_msg><![CDATA[支付失败]]></return_msg>
</xml>";
            ContentResult err = new ContentResult { Content = errStr };
            string rightStr = @"
<xml>
    <return_code><![CDATA[SUCCESS]]></return_code>
    <return_msg><![CDATA[支付成功]]></return_msg>
</xml>";
            ContentResult right = new ContentResult { Content = rightStr };

            // 设置appkey,用于验证参数签名
            resHandler.SetKey(wxSettings.PayV3Info.Key);
            // 参数签名验证
            if (!resHandler.IsTenpaySign())
            {
                logger.LogError($"微信支付回调:签名错误,result:{xml}");
                return err;
            }
            // 支付回调结果是否正确
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
            {
                logger.LogError($"微信支付回调,支付失败:return error,result:{xml}");
                var callbacks = HttpContext.RequestServices.GetServices<IPaySuccessNotify>();
                if (!callbacks.IsNullOrEmpty())
                    foreach (var item in callbacks.OrderBy(r => r.Priority))
                        await item.Handle(result);
                return err;
            }

            try
            {
                var callbacks = HttpContext.RequestServices.GetServices<IPaySuccessNotify>();
                if (!callbacks.IsNullOrEmpty())
                    foreach (var item in callbacks.OrderBy(r => r.Priority))
                        await item.Handle(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"微信支付回调逻辑处理错误:{xml}");
                return err;
            }
            return right;
        }
    }
}