namespace Shashlik.EfCore.Audit;

public interface IAudit : IAuditDelete, IAuditCreate, IAuditUpdate
{
}