using Guc.NLogger.Loggers;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Sbt.Common;
using Sbt.Enterprise;
using Sbt.Identity;
using Sbt.Identity.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sbt.Ids4
{
    public class ProfileService : IProfileService
    {
        UserManager UserManager { get; }

        EntManager EntManager { get; }
        ILogger<LoginLogs> Logger { get; }

        public ProfileService(
            UserManager userManager,
            EntManager entManager,
            ILogger<LoginLogs> logger)
        {
            UserManager = userManager;
            EntManager = entManager;
            Logger = logger;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims.ToList();
            var user = await GetUser(context.Subject);

            claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
            var roles = await UserManager.GetRolesAsync(_user);
            if (roles.Count == 0)
                throw new Exception("user 没有角色 数据, userId:" + user.Id);
            claims.Add(new Claim(JwtClaimTypes.Role, roles[0]));

            if (roles.Contains(Globals.Roles.EntAdmin) || roles.Contains(Globals.Roles.EntCw))
            {
                var ent = await EntManager.GetEntByUser(user.Id);
                if (ent == null)
                    throw new Exception($"企业user没有企业数据,userId: {user.Id}");
                claims.Add(new Claim("entid", ent.Id.ToString()));
            }

            // 这里是设置token包含的用户属性claim
            context.IssuedClaims = claims;
            Logger.LogInformation($"[{context.Client.ClientId}]:{user.UserName},用户登录日志");
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            _user = await GetUser(context.Subject);
            context.IsActive = !_user.IsDeleted && !_user.Disabled;
        }

        private async Task<Users> GetUser(ClaimsPrincipal claimsPrincipal)
        {
            if (_user == null)
            {
                // sub属性就是用户id
                var userId = claimsPrincipal.GetSubjectId();
                // 查找用户
                _user = await UserManager.FindByIdAsync(userId);
            }

            return _user;
        }

        private Users _user;
    }
}
