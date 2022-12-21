using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.EfCore.Audit;

/// <summary>
/// ef core 自动审计
/// </summary>
public class AuditingInterceptor : SaveChangesInterceptor
{
    public AuditingInterceptor()
    {
        CurrentUserContext = new Lazy<ICurrentUserContext>(() =>
            GlobalKernelServiceProvider.KernelServiceProvider!.GetRequiredService<ICurrentUserContext>());
    }

    private Lazy<ICurrentUserContext> CurrentUserContext { get; }


    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;
        var currentUserInfo = CurrentUserContext.Value.GetCurrentUserInfo();
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    Delete(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
                case EntityState.Modified:
                    Update(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
                case EntityState.Added:
                    Add(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
            }
        }

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return ValueTask.FromResult(result);
        var currentUserInfo = CurrentUserContext.Value.GetCurrentUserInfo();


        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    Delete(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
                case EntityState.Modified:
                    Update(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
                case EntityState.Added:
                    Add(entry, currentUserInfo.userId, currentUserInfo.user);
                    break;
            }
        }


        return ValueTask.FromResult(result);
    }

    private void Delete(EntityEntry entry, string userId, string user)
    {
        if (entry.Entity is not IAuditDelete entity) return;
        entry.State = EntityState.Modified;
        entity.DeleteUserId = userId;
        entity.DeleteUser = user;
        entity.DeleteTime = DateTime.Now;
        entity.IsDeleted = true;
    }

    private void Add(EntityEntry entry, string userId, string user)
    {
        if (entry.Entity is not IAuditCreate entity) return;
        entity.CreateUserId = userId;
        entity.CreateUser = user;
        entity.CreateTime = DateTime.Now;
        Update(entry, userId, user);
    }

    private void Update(EntityEntry entry, string userId, string user)
    {
        if (entry.Entity is not IAuditUpdate entity) return;
        entity.UpdateUserId = userId;
        entity.UpdateUser = user;
        entity.UpdateTime = DateTime.Now;
    }
}