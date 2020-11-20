using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Shashlik.Identity
{
    public class ShashlikRoleManager<TRole, TKey> : RoleManager<TRole>, IShashlikRoleManager
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>, IIdentityRole

    {
        public ShashlikRoleManager(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }

        private TRole AsRole(IIdentityRole role)
        {
            return (role as TRole)!;
        }

        public Task<IdentityResult> AddClaimAsync(IIdentityRole role, Claim claim)
        {
            return base.AddClaimAsync(AsRole(role), claim);
        }

        public Task<IdentityResult> CreateAsync(IIdentityRole role)
        {
            return base.CreateAsync(AsRole(role));
        }

        public Task<IdentityResult> DeleteAsync(IIdentityRole role)
        {
            return base.DeleteAsync(AsRole(role));
        }

        public async Task<IIdentityRole> FindIdentityRoleByIdAsync(string roleId)
        {
            return await base.FindByIdAsync(roleId);
        }

        public async Task<IIdentityRole> FindIdentityRoleByNameAsync(string roleName)
        {
            return await base.FindByNameAsync(roleName);
        }

        public Task<IList<Claim>> GetClaimsAsync(IIdentityRole role)
        {
            return base.GetClaimsAsync(AsRole(role));
        }

        public Task<string> GetRoleIdAsync(IIdentityRole role)
        {
            return base.GetRoleIdAsync(AsRole(role));
        }

        public Task<string> GetRoleNameAsync(IIdentityRole role)
        {
            return base.GetRoleNameAsync(AsRole(role));
        }

        public Task<IdentityResult> RemoveClaimAsync(IIdentityRole role, Claim claim)
        {
            return base.RemoveClaimAsync(AsRole(role), claim);
        }

        public Task<IdentityResult> SetRoleNameAsync(IIdentityRole role, string name)
        {
            return base.SetRoleNameAsync(AsRole(role), name);
        }

        public Task<IdentityResult> UpdateAsync(IIdentityRole role)
        {
            return base.UpdateAsync(AsRole(role));
        }

        public Task UpdateNormalizedRoleNameAsync(IIdentityRole role)
        {
            return base.UpdateNormalizedRoleNameAsync(AsRole(role));
        }
    }
}