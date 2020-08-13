using Guc.Utils.Extensions;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.Wx
{
    public interface IWxPay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="title"></param>
        /// <param name="totalAmount"></param>
        /// <param name="attach"></param>
        /// <param name="orderId"></param>
        /// <param name="localTradeNo"></param>
        /// <param name="clientIp"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        Task<PrePayResult<AppPayResult>> BuildApp(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string clientIp, string notifyUrl, TimeSpan? expire = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="title"></param>
        /// <param name="totalAmount"></param>
        /// <param name="attach"></param>
        /// <param name="orderId"></param>
        /// <param name="localTradeNo"></param>
        /// <param name="clientIp"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="redirect_url"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        Task<PrePayResult<H5PayResult>> BuildH5(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string clientIp, string notifyUrl, string redirect_url, TimeSpan? expire = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="title"></param>
        /// <param name="totalAmount"></param>
        /// <param name="attach"></param>
        /// <param name="orderId"></param>
        /// <param name="localTradeNo"></param>
        /// <param name="openId"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        Task<PrePayResult<JsApiResult>> BuildJsApi(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string openId, string notifyUrl, TimeSpan? expire = null);

        /// <summary>
        /// 
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
        Task<PrePayResult<QrCodeResult>> BuildWxPayQRCode(string appId, string mchId, string mchKey, string title, int totalAmount,
            string attach, string orderId, string localTradeNo, string notifyUrl, TimeSpan? expire = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="localTranNo"></param>
        /// <param name="localRefundNo"></param>
        /// <param name="orderAmount"></param>
        /// <param name="refundDesc"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<bool> Refund(string appId, string mchId, string mchKey, string localTranNo, string localRefundNo,
            int orderAmount, int refundAmount, string refundDesc, string notifyUrl);

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="localRefundNo"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool TryRefundQuery(string appId, string mchId, string mchKey, string localTranNo, out WxRefundData wxRefundData, int offset = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchKey"></param>
        /// <param name="localTradeNo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryOrderQuery(string appId, string mchId, string mchKey, string localTradeNo, out OrderQueryResult result);
    }


    public class RefundQueryResultInner : RefundQueryResult
    {
        public List<WxRefundListData> List { get; set; }

        public RefundQueryResultInner(string resultXml) : base(resultXml)
        {
            var refundNoList = GetXmlValuesInner<string>("out_refund_no_");
            var refundIdList = GetXmlValuesInner<string>("refund_id_");
            var refundChannelList = GetXmlValuesInner<string>("refund_channel_");
            var refundFeeList = GetXmlValuesInner<int>("refund_fee_");
            var realFefundFeeList = GetXmlValuesInner<int>("settlement_refund_fee_");
            var statusList = GetXmlValuesInner<string>("refund_status_");
            var time = GetXmlValuesInner<string>("refund_success_time_");

            for (int i = 0; i < refundNoList.Count; i++)
            {
                List.Add(new WxRefundListData
                {
                    LocalRefundNo = refundNoList[i],
                    RefundAmount = refundFeeList[i],
                    RealRefundAmount = realFefundFeeList[i],
                    RefundChannel = refundChannelList[i],
                    RefundId = refundIdList[i],
                    Status = Enum.Parse<WxRefundListData.RefundStatus>(statusList[i]),
                    SuccessTime = time[i].IsNullOrWhiteSpace() ? null : (long?)time[i].ConvertTo<DateTime>().GetLongDate()
                });
            }
        }

        /// <summary>
        /// 获取Xml结果中对应节点的集合值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public Dictionary<int, T> GetXmlValuesInner<T>(string nodeName)
        {
            var result = new Dictionary<int, T>();
            try
            {
                if (_resultXml != null)
                {
                    var xElement = _resultXml.Element("xml");
                    if (xElement != null)
                    {
                        var nodeList = xElement
                            .Elements()
                            .Where(z => z.Name.ToString().StartsWith(nodeName));

                        nodeList.Foreach(z =>
                           {
                               try
                               {
                                   if (z.Name.ToString().Substring(nodeName.Length).TryParse(out int index))
                                   {
                                       var t = (z.Value as IConvertible).ConvertTo<T>();
                                       result.Add(index, t);
                                   }
                               }
                               catch (Exception)
                               {
                                   throw;
                               }
                           });
                    }
                }
            }
            catch (System.Exception)
            {
                result = null;
            }
            return result;
        }
    }

    public class WxRefundData
    {
        /// <summary>
        /// 订单金额
        /// </summary>
        public long TotalAmount { get; set; }

        /// <summary>
        /// 总退款次数
        /// </summary>
        public int TotalRefundCount { get; set; }

        /// <summary>
        /// 退款详情列表
        /// </summary>
        public List<WxRefundListData> List { get; set; }

        /// <summary>
        /// 总已退款金额
        /// </summary>
        public long TotalRefundAmount => List?.Where(r => r.Status == WxRefundListData.RefundStatus.SUCCESS)?.Sum(r => r.RefundAmount) ?? 0;

        /// <summary>
        /// 总已退款实际金额(去掉代金券之类的)
        /// </summary>
        public long TotalRealRefundAmount => List?.Where(r => r.Status == WxRefundListData.RefundStatus.SUCCESS)?.Sum(r => r.RealRefundAmount) ?? 0;
    }

    /// <summary>
    /// 退款数据
    /// </summary>
    public class WxRefundListData
    {
        /// <summary>
        /// 本地退款交易号
        /// </summary>
        public string LocalRefundNo { get; set; }

        /// <summary>
        /// 退款id
        /// </summary>
        public string RefundId { get; set; }

        /// <summary>
        /// 退款通道，
        /// ORIGINAL—原路退款，BALANCE—退回到余额，OTHER_BALANCE—原账户异常退到其他余额账户，OTHER_BANKCARD—原银行卡异常退到其他银行卡
        /// </summary>
        public string RefundChannel { get; set; }

        /// <summary>
        /// 退款金额,refund_fee_$n
        /// </summary>
        public int RefundAmount { get; set; }

        /// <summary>
        /// 退款金额,settlement_refund_fee_$n
        /// </summary>
        public int RealRefundAmount { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundStatus Status { get; set; }

        /// <summary>
        /// 退款成功时间
        /// </summary>
        public long? SuccessTime { get; set; }

        public enum RefundStatus
        {
            /// <summary>
            /// 退款成功
            /// </summary>
            SUCCESS = 1,
            /// <summary>
            /// 退款关闭
            /// </summary>
            REFUNDCLOSE = 2,
            /// <summary>
            /// 处理中
            /// </summary>
            PROCESSING = 3,
            /// <summary>
            /// 退款异常
            /// </summary>
            CHANGE = 4
        }
    }

}