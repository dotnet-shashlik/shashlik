using System;

namespace Shashlik.EfCore.Audit;

/// <summary>
/// 新增审计
/// </summary>
public interface IAuditCreate
{
    /// <summary>
    /// 创建人员id
    /// </summary>
    public string? CreateUserId { get; set; }

    /// <summary>
    /// 创建人员
    /// </summary>
    public string? CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}