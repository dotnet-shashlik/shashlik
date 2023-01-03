using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Audit;

/// <summary>
/// 默认用户信息上下文,从HttpContext中读取当前用户信息
/// </summary>
[Singleton]
public class DefaultCurrentUserContext : ICurrentUserContext
{
    public DefaultCurrentUserContext(IServiceProvider serviceProvider)
    {
        HttpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
    }

    private IHttpContextAccessor? HttpContextAccessor { get; }

    public (string? userId, string? user) GetCurrentUserInfo()
    {
        if (HttpContextAccessor is null)
            return (null, null);
        return
        (
            HttpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            HttpContextAccessor.HttpContext?.User.Identity?.Name
        );
    }
}