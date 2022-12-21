using System;

namespace Shashlik.EfCore.Audit;

/// <summary>
/// 更新审计
/// </summary>
public interface IAuditUpdate
{
    /// <summary>
    /// 更新人员id
    /// </summary>
    public string? UpdateUserId { get; set; }

    /// <summary>
    /// 更新人员
    /// </summary>
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}