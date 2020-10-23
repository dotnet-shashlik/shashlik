namespace Shashlik.Ids4.Identity
{
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

        /// <summary>
        /// 过期时间
        /// </summary>
        public long CreateTime { get; set; }
    }
}