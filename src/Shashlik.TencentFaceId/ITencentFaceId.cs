using Microsoft.Extensions.Options;
using TencentCloud.Faceid.V20180301.Models;

namespace TencentFaceId.Sdk
{
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    public interface ITencentFaceId
    {
        /// <summary>
        /// 获取人身核验数据
        /// </summary>
        /// <returns></returns>
        DetectAuthResponse DetectAuthH5(string redirect);

        /// <summary>
        /// 获取人身核验信息
        /// </summary>
        /// <param name="bizToken"></param>
        /// <returns></returns>
        DetectInfo GetDetectInfo(string bizToken);

        /// <summary>
        /// 两要素认证
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public (bool success, string desc) TwoElementCertificate(string idCard, string realName);
        public (bool success, string desc) FourElementCertificate(string idCard, string realName, string phone, string bankCard);
    }

    public class EmptyFaceId : DefaultTencentFaceId
    {
        public new (bool success, string desc) TwoElementCertificate(string idCard, string realName)
        {
            return (true, "");
        }

        public EmptyFaceId(IOptions<TencentFaceIdOptions> options) : base(options)
        {
        }
    }
}
