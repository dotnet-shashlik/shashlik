using System.Threading.Tasks;

namespace Shashlik.Ids4.Identity.Extend
{
    public interface IIdentitySmsSender
    {
        Task Send(string purpose, string email, string code);
    }
}