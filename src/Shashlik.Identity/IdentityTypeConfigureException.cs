using System;

namespace Shashlik.Identity
{
    public class IdentityTypeConfigureException : Exception
    {
        public IdentityTypeConfigureException()
            : base($"Make sure implement of interface \"{nameof(IIdentityTypeConfigure)}<,,>\"")
        {
        }
    }
}