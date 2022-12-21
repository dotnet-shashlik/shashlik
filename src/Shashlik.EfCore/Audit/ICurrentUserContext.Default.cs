using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Audit;

/// <summary>
/// 默认用户信息上下文,从HttpContext中读取当前用户信息
/// </summary>
[Scoped]
public class DefaultCurrentUserContext : ICurrentUserContext
{
    public DefaultCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    private IHttpContextAccessor HttpContextAccessor { get; }

    public (string? userId, string? user) GetCurrentUserInfo()
    {
        return
        (
            HttpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            HttpContextAccessor.HttpContext?.User.Identity?.Name
        );
    }
}