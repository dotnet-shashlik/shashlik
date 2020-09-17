using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Identity.HttpApi
{
    /// <summary>
    /// identity 邮件发送接口类,自动注册为单例
    /// </summary>
    public interface IIdentityEmailSender : ISingleton
    {
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="purpose">验证目的</param>
        /// <param name="email">邮件地址</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task Send(string purpose, string email, string code);
    }
}