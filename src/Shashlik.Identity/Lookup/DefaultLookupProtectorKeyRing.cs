using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;

namespace Shashlik.Identity.Lookup
{
    public class DefaultLookupProtectorKeyRing : ILookupProtectorKeyRing
    {
        public DefaultLookupProtectorKeyRing(IKeyManager keyManager, IKeyRingProvider keyRingProvider)
        {
            KeyManager = keyManager;
            KeyRingProvider = keyRingProvider;
        }

        private IKeyManager KeyManager { get; }
        private IKeyRingProvider KeyRingProvider { get; }

        public IEnumerable<string> GetAllKeyIds()
        {
            return KeyManager.GetAllKeys()
                .Select(r => r.KeyId.ToString())
                .ToList();
        }

        public string CurrentKeyId => KeyRingProvider.GetCurrentKeyRing().DefaultKeyId.ToString();

        public string this[string keyId] => keyId;
    }
}