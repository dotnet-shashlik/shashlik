using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Identity.AspNetCore
{
    [AutoOptions("Shashlik:Identity")]
    public class ShashlikAspNetIdentityOptions
    {
        /// <summary>
        /// identity 原生配置
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; } = new IdentityOptions();
    }
}