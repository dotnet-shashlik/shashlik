using System.Threading.Tasks;

namespace Shashlik.Ids4.Identity.Extend
{
    public interface IIdentityEMailSender
    {
        Task Send(string purpose, string email, string code);
    }
}