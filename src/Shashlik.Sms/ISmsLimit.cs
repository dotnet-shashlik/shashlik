using Shashlik.Kernel.Dependency;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送频率限制
    /// </summary>
    public interface ISmsLimit
    {
        /// <summary>
        /// 发送频率限制检查
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        bool CanSend(string phone);

        /// <summary>
        /// 更新限制数据
        /// </summary>
        /// <param name="phone"></param>
        void SendDone(string phone);
    }
}