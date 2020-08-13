using Guc.Utils;
using Guc.Utils.Common;
using Guc.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Guc.Wx
{
    public class DefaultWxPay : Guc.Kernel.Dependency.ISingleton, IWxPay
    {
        ILogger<DefaultWxPay> logger { get; }
        public DefaultWxPay(
            ILogger<DefaultWxPay> logger
            )
        {
            this.logger = logger;
        }

        /// <summary>
        /// 二维码支付
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="title"></param>
        /// <param name="totalAmount"></param>
        /// <param name="attach"></param>
        /// <param name="orderId"></param>
        /// <param name="localTradeNo"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public async Task<PrePayResult<QrCodeResult>> BuildWxPayQRCode(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string notifyUrl, TimeSpan? expire = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("支付标题不能为空", nameof(title));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单id不能为空", nameof(orderId));
            if (totalAmount < 1)
                throw new ArgumentException("支付金额最低0.01元", nameof(totalAmount));

            string nonceStr = TenPayV3Util.GetNoncestr();

            // 微信支付统一订单 api调用,生成预付单
            var requestData = new TenPayV3UnifiedorderRequestData(
                      appId,
                      mchId,
                      title,
                      localTradeNo,
                      totalAmount,
                      null,
                      notifyUrl,
                      TenPayV3Type.NATIVE,
                      null,
                      mchKey,
                      nonceStr,
                      attach: attach,
                      productId: orderId,
                      timeExpire: DateTime.Now.Add(expire ?? TimeSpan.FromHours(1))
                      );

            var result = await TenPayV3.UnifiedorderAsync(requestData);
            logger.LogInformation($"NATIVE支付预付单结果:{result.ResultXml}");
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
                throw new Exception($"生成微信预付单失败,appid:{appId},request:{JsonHelper.Serialize(requestData)},result:{result.ResultXml}");

            return new PrePayResult<QrCodeResult>
            {
                Result = new QrCodeResult
                {
                    QrcodeUrl = result.code_url
                },
                LocalTradeNo = localTradeNo,
                UnifiedorderResult = result
            };
        }

        /// <summary>
        /// 生成jsapi支付预付单(JASPI)
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="title">商品标题</param>
        /// <param name="totalAmount">支付金额(单位分)</param>
        /// <param name="attach">附加数据</param>
        /// <param name="orderId">订单id</param>
        /// <param name="openId">用户openid</param>
        /// <param name="notifyUrl">支付完成通知地址,为空则使用配置中的</param>
        /// <returns></returns>
        public async Task<PrePayResult<JsApiResult>> BuildJsApi(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string openId, string notifyUrl, TimeSpan? expire = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("支付标题不能为空", nameof(title));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单id不能为空", nameof(orderId));
            if (totalAmount < 1)
                throw new ArgumentException("支付金额最低0.01元", nameof(totalAmount));

            string nonceStr = TenPayV3Util.GetNoncestr();

            // 微信支付统一订单 api调用,生成预付单
            var requestData = new TenPayV3UnifiedorderRequestData(
                      appId,
                      mchId,
                      title,
                      localTradeNo,
                      totalAmount,
                      null,
                      notifyUrl,
                      TenPayV3Type.JSAPI,
                      openId,
                      mchKey,
                      nonceStr,
                      attach: attach,
                      productId: orderId,
                      timeExpire: DateTime.Now.Add(expire ?? TimeSpan.FromHours(1))
                      );

            var result = await TenPayV3.UnifiedorderAsync(requestData);
            logger.LogInformation($"JASPI支付预付单结果:{result.ResultXml}");
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
                throw new Exception($"生成微信预付单失败,appid:{appId},request:{JsonHelper.Serialize(requestData)},result:{result.ResultXml}");

            // 返回前端支付jaapi支付数据
            var timeStamp = TenPayV3Util.GetTimestamp();
            string nonceStrSign = TenPayV3Util.GetNoncestr();
            var package = $"prepay_id={result.prepay_id}";
            var sign = TenPayV3.GetJsPaySign(appId, timeStamp, nonceStrSign, package, mchKey);

            return new PrePayResult<JsApiResult>
            {
                Result = new JsApiResult
                {
                    AppId = appId,
                    NonceStr = nonceStrSign,
                    Package = package,
                    PaySign = sign,
                    SignType = "MD5",
                    TimeStamp = timeStamp
                },
                UnifiedorderResult = result,
                LocalTradeNo = localTradeNo
            };
        }

        /// <summary>
        /// app支付预付单
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="title">商品标题</param>
        /// <param name="totalAmount">支付金额(单位分)</param>
        /// <param name="attach">附加数据</param>
        /// <param name="orderId">订单id</param>
        /// <param name="clientIp">客户端ip</param>
        /// <param name="notifyUrl">支付完成通知地址,为空则使用配置中的</param>
        /// <returns></returns>
        public async Task<PrePayResult<AppPayResult>> BuildApp(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string clientIp, string notifyUrl, TimeSpan? expire = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("支付标题不能为空", nameof(title));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单id不能为空", nameof(orderId));
            if (totalAmount < 1)
                throw new ArgumentException("支付金额最低0.01元", nameof(totalAmount));

            string nonceStr = TenPayV3Util.GetNoncestr();
            // 微信支付统一订单 api调用,生成预付单
            var requestData = new TenPayV3UnifiedorderRequestData(
                      appId,
                      mchId,
                      title,
                      localTradeNo,
                      totalAmount,
                      clientIp,
                      notifyUrl,
                      TenPayV3Type.APP,
                      null,
                      mchKey,
                      nonceStr,
                      attach: attach,
                      productId: orderId,
                      timeExpire: DateTime.Now.Add(expire ?? TimeSpan.FromHours(1))
                      );

            var result = await TenPayV3.UnifiedorderAsync(requestData);
            logger.LogInformation($"APP支付预付单结果:{result.ResultXml}");
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
                throw new Exception($"生成微信预付单失败,appid:{appId},request:{JsonHelper.Serialize(requestData)},result:{result.ResultXml}");

            var timeStamp = TenPayV3Util.GetTimestamp();
            string nonceStrSign = TenPayV3Util.GetNoncestr(); ;
            var package = $"Sign=WXPay";

            // 生成签名sign
            RequestHandler paySignReqHandler = new RequestHandler(null);
            paySignReqHandler.SetParameter("appid", appId);
            paySignReqHandler.SetParameter("timestamp", timeStamp);
            paySignReqHandler.SetParameter("noncestr", nonceStrSign);
            paySignReqHandler.SetParameter("package", package);
            paySignReqHandler.SetParameter("prepayid", result.prepay_id);
            paySignReqHandler.SetParameter("partnerid", mchId);
            var paySign = paySignReqHandler.CreateMd5Sign("key", mchKey);

            return new PrePayResult<AppPayResult>
            {
                Result = new AppPayResult
                {
                    Appid = appId,
                    Noncestr = nonceStrSign,
                    Prepayid = result.prepay_id,
                    Package = package,
                    Partnerid = mchId,
                    Sign = paySign,
                    Timestamp = timeStamp
                },
                UnifiedorderResult = result,
                LocalTradeNo = localTradeNo
            };
        }

        /// <summary>
        /// H5支付预付单
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="title">商品标题</param>
        /// <param name="totalAmount">支付金额(单位分)</param>
        /// <param name="attach">附加数据</param>
        /// <param name="orderId">订单id</param>
        /// <param name="openId">用户openid</param>
        /// <param name="clientIp">客户端ip(微信安全校验)</param>
        /// <param name="notifyUrl">支付完成通知地址,为空则使用配置中的</param>
        /// <param name="redirect_url">前端跳转地址</param>
        /// <returns></returns>
        public async Task<PrePayResult<H5PayResult>> BuildH5(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string clientIp, string notifyUrl, string redirect_url, TimeSpan? expire = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("支付标题不能为空", nameof(title));
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单id不能为空", nameof(orderId));
            if (totalAmount < 1)
                throw new ArgumentException("支付金额最低0.01元", nameof(totalAmount));

            string nonceStr = TenPayV3Util.GetNoncestr();
            // 微信支付统一订单 api调用,生成预付单
            var requestData = new TenPayV3UnifiedorderRequestData(
                      appId,
                      mchId,
                      title,
                      localTradeNo,
                      totalAmount,
                      clientIp,
                      notifyUrl,
                      TenPayV3Type.MWEB,
                      null,
                      mchKey,
                      nonceStr,
                      attach: attach,
                      productId: orderId,
                      timeExpire: DateTime.Now.Add(expire ?? TimeSpan.FromHours(1))
                      );

            var result = await TenPayV3.UnifiedorderAsync(requestData);
            logger.LogInformation($"MWEB支付预付单结果:{result.ResultXml}");
            if (!result.IsReturnCodeSuccess() || !result.IsResultCodeSuccess())
                throw new Exception($"生成微信预付单失败,appid:{appId},request:{JsonHelper.Serialize(requestData)},result:{result.ResultXml}");

            string mWebUrl = result.mweb_url;
            if (redirect_url.IsNullOrWhiteSpace())
            {
                if (!HttpUtility.UrlDecode(redirect_url).IsMatch(Consts.Regexs.Url))
                    throw new ArgumentException("跳转参数错误", nameof(redirect_url));

                mWebUrl += string.Format("&redirect_url={0}", redirect_url);
            }

            return new PrePayResult<H5PayResult>
            {
                Result = new H5PayResult
                {
                    MwebUrl = mWebUrl
                },
                LocalTradeNo = localTradeNo,
                UnifiedorderResult = result
            };
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="localTradeNo">系统交易单号(预付单返回的OutTradeNo)</param>
        /// <returns></returns>
        public bool TryOrderQuery(string appId, string mchId, string mchKey, string localTradeNo, out OrderQueryResult result)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            result = TenPayV3.OrderQuery(new TenPayV3OrderQueryRequestData(
               appId,
               mchId,
               null,
               nonceStr,
               localTradeNo,
               mchKey));

            logger.LogInformation($"微信支付订单查询,out_trade_no:{localTradeNo},result:{result.ResultXml}");
            return result.IsReturnCodeSuccess() && result.IsResultCodeSuccess();
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="localTranNo"></param>
        /// <param name="localRefundNo"></param>
        /// <param name="orderAmount"></param>
        /// <param name="refundAmount"></param>
        /// <param name="refundDesc"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public async Task<bool> Refund(string appId, string mchId, string mchKey, string localTranNo, string localRefundNo,
            int orderAmount, int refundAmount, string refundDesc, string notifyUrl)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            var result = await TenPayV3.RefundAsync(Guc.Kernel.KernelServiceProvider.ServiceProvider, new TenPayV3RefundRequestData(
                appId, mchId, mchKey, null, nonceStr, null, localTranNo, localRefundNo,
                (int)orderAmount, (int)refundAmount, null, null, refundDescription: refundDesc, notifyUrl: notifyUrl));

            logger.LogInformation($"微信支付申请退款,out_trade_no:{localTranNo},result:{result.ResultXml}");
            return result.IsReturnCodeSuccess() && result.IsResultCodeSuccess();
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="localTranNo"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool TryRefundQuery(string appId, string mchId, string mchKey, string localTranNo, out WxRefundData wxRefundData, int offset = 0)
        {
            wxRefundData = null;

            string nonceStr = TenPayV3Util.GetNoncestr();
            var result = TenPayV3.RefundQuery(new TenPayV3RefundQueryRequestData(
                appId, mchId, mchKey, nonceStr, null, null, localTranNo, null, null, offset: offset));

            logger.LogInformation($"微信支付退款查询,localRefundNo:{localTranNo},result:{result.ResultXml}");
            if (!result.IsReturnCodeSuccess())
            {
                logger.LogWarning($"微信退款查询异常，localRefundNo:{localTranNo},result:{result.ResultXml}");
                return false;
            }
            if (!result.IsResultCodeSuccess())
            {
                logger.LogWarning($"微信退款查询结果错误，localRefundNo:{localTranNo},result:{result.ResultXml}");
                return false;
            }
            try
            {
                var resInner = new RefundQueryResultInner(result.ResultXml);
                wxRefundData = new WxRefundData
                {
                    TotalAmount = resInner.total_fee.ConvertTo<long>(),
                    TotalRefundCount = resInner.refund_count.ConvertTo<int>(),
                    List = resInner.List
                };

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"微信退款查询结果处理异常，localRefundNo:{localTranNo},result:{result.ResultXml}", ex);
            }
        }
    }
}
