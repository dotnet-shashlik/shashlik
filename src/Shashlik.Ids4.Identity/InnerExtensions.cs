using System.Collections.Generic;
using IdentityServer4.Validation;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    public static class InnerExtensions
    {
        public static void WriteError(this ExtensionGrantValidationContext context, ErrorCodes code)
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = ((int) ErrorCodes.RequiresTwoFactor).ToString(),
                ErrorDescription = ErrorCodes.RequiresTwoFactor.GetEnumDescription(),
                CustomResponse = new Dictionary<string, object?>
                {
                    {"code", (int) code},
                    {"message", code.GetEnumDescription()},
                }
            };
        }

        public static void WriteError(this ResourceOwnerPasswordValidationContext context, ErrorCodes code)
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = ((int) ErrorCodes.RequiresTwoFactor).ToString(),
                ErrorDescription = ErrorCodes.RequiresTwoFactor.GetEnumDescription(),
                CustomResponse = new Dictionary<string, object?>
                {
                    {"code", (int) code},
                    {"message", code.GetEnumDescription()},
                }
            };
        }
    }
}