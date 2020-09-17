using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Identity.HttpApi
{
    /// <summary>
    /// identity 短信发送接口类,自动注册为单例
    /// </summary>
    public interface IIdentitySmsSender : ISingleton
    {
        Task Send(string purpose, string phone, string code);
    }
}