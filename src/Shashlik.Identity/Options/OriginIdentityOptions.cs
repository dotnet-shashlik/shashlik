using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Identity.Options
{
    [AutoOptions("Shashlik.Identity.IdentityOptions")]
    public class OriginIdentityOptions : IdentityOptions
    {
    }
}