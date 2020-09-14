using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;

namespace Shashlik.Ids4.Identity
{
    public class Ids4IdentityConfigure : IIdentityServerBuilderConfigure
    {
        public Ids4IdentityConfigure(IOptions<Ids4IdentityOptions> options)
        {
            Options = options.Value;
        }

        private Ids4IdentityOptions Options { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!Options.Enable)
                return;

            builder.AddAspNetIdentity<Users>();
            if (Options.AllowValidator.Contains(Ids4IdentityGrantTypes.Password))
                builder.Services.Replace(
                    ServiceDescriptor.Transient<IResourceOwnerPasswordValidator, PasswordValidator<Users>>());
            if (Options.AllowValidator.Contains(Ids4IdentityGrantTypes.Phone))
                builder.AddExtensionGrantValidator<PhoneValidator>();
            if (Options.AllowValidator.Contains(Ids4IdentityGrantTypes.EMail))
                builder.AddExtensionGrantValidator<EMailValidator>();
        }
    }
}