namespace Shashlik.EfCore.Audit;

public interface ICurrentUserContext
{
    public (string? userId, string? user) GetCurrentUserInfo();
}