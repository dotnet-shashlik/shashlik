namespace Shashlik.Identity
{
    public interface IIdentityRole
    {
        string IdString { get; }

        string Name { get; set; }

        string NormalizedName { get; set; }

        string ConcurrencyStamp { get; set; }
    }
}