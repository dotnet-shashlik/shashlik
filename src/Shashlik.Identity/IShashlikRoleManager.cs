using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Shashlik.Identity
{
    public interface IShashlikRoleManager
    {
        ILookupNormalizer KeyNormalizer { get; set; }


        IdentityErrorDescriber ErrorDescriber { get; set; }


        ILogger Logger { get; set; }


        bool SupportsQueryableRoles { get; }


        bool SupportsRoleClaims { get; }


        Task<IdentityResult> AddClaimAsync(IIdentityRole role, Claim claim);


        Task<IdentityResult> CreateAsync(IIdentityRole role);


        Task<IdentityResult> DeleteAsync(IIdentityRole role);


        Task<IIdentityRole> FindIdentityRoleByIdAsync(string roleId);


        Task<IIdentityRole> FindIdentityRoleByNameAsync(string roleName);


        Task<IList<Claim>> GetClaimsAsync(IIdentityRole role);


        Task<string> GetRoleIdAsync(IIdentityRole role);


        Task<string> GetRoleNameAsync(IIdentityRole role);


        string NormalizeKey(string key);


        Task<IdentityResult> RemoveClaimAsync(IIdentityRole role, Claim claim);


        Task<bool> RoleExistsAsync(string roleName);


        Task<IdentityResult> SetRoleNameAsync(IIdentityRole role, string name);


        Task<IdentityResult> UpdateAsync(IIdentityRole role);


        Task UpdateNormalizedRoleNameAsync(IIdentityRole role);
    }
}