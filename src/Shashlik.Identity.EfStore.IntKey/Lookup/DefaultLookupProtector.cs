using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace Shashlik.Identity.Lookup
{
    public class DefaultLookupProtector : ILookupProtector
    {
        public DefaultLookupProtector(IDataProtectionProvider dataProtectionProvider)
        {
            DataProtectionProvider = dataProtectionProvider;
        }

        private IDataProtectionProvider DataProtectionProvider { get; }

        public string Protect(string keyId, string data)
        {
            return DataProtectionProvider.CreateProtector($"lookup_protector")
                .Protect(data);
        }

        public string Unprotect(string keyId, string data)
        {
            return DataProtectionProvider.CreateProtector($"lookup_protector")
                .Unprotect(data);
        }
    }
}