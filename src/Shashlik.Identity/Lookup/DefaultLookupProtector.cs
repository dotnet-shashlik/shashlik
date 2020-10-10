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
            return Encoding.UTF8.GetString(DataProtectionProvider.CreateProtector($"lookup_protector")
                .Protect(Encoding.UTF8.GetBytes(data)));
        }

        public string Unprotect(string keyId, string data)
        {
            return Encoding.UTF8.GetString(DataProtectionProvider.CreateProtector($"lookup_protector")
                .Unprotect(Encoding.UTF8.GetBytes(data)));
        }
    }
}