using Shashlik.Kernel.Dependency;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送频率限制
    /// </summary>
    [Singleton]
    public interface ISmsLimit
    {
        /// <summary>
        /// 发送频率限制检查
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="subject">短信类型</param>
        /// <returns></returns>
        bool LimitCheck(string phone, string subject);

        /// <summary>
        /// 更新限制数据
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="subject"></param>
        void UpdateLimit(string phone, string subject);
    }
}