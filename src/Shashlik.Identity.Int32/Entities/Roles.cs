using Microsoft.AspNetCore.Identity;

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Roles : IdentityRole<int>, IIdentityRole
    {
        public string IdString => Id.ToString();
    }
}