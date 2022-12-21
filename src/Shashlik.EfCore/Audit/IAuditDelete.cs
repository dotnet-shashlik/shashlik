using System;
using Shashlik.EfCore.Filter;

namespace Shashlik.EfCore.Audit;

public interface IAuditDelete : ISoftDeleted<DateTime>
{
    /// <summary>
    /// 删除人员id
    /// </summary>
    public string? DeleteUserId { get; set; }

    /// <summary>
    /// 删除人员
    /// </summary>
    public string? DeleteUser { get; set; }
}