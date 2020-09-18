using System.Collections.Specialized;
using System.Threading.Tasks;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Ids4.Identity.Interfaces;
using Shashlik.Utils.Extensions;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace Shashlik.Ids4.Identity.Inner
{
    public class EmailCaptchaUserGetter : ICaptchaUserGetter
    {
        public string Type => "email";

        public int Validate(string unifier)
        {
            if (unifier.IsNullOrWhiteSpace())
                return ShashlikIds4IdentityConsts.ErrorCodes.EmailError;
            if (!unifier.IsMatch(Utils.Consts.Regexs.Email))
                return ShashlikIds4IdentityConsts.ErrorCodes.EmailError;

            return 0;
        }

        public async Task<Users> FindByUnifierAsync(string unifier, ShashlikUserManager manager,
            NameValueCollection postData)
        {
            return await manager.FindByEmailAsync(unifier);
        }
    }
}