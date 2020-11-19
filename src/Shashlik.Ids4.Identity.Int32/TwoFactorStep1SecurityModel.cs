#pragma warning disable 8618
namespace Shashlik.Ids4.Identity.Int32
{
    /// <summary>
    /// 两步验证,第一步加密数据模型
    /// </summary>
    public class TwoFactorStep1SecurityModel
    {
        /// <summary>
        /// userid
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 随机值
        /// </summary>
        public string Nonce { get; set; }
    }
}