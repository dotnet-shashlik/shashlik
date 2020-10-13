namespace Shashlik.Ids4.Identity
{
    public class TwoFactorStep1SecurityModel
    {
        /// <summary>
        /// userid
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long Expiration { get; set; }
    }
}