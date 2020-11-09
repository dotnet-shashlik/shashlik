using Shashlik.Kernel.Attributes;

namespace Shashlik.EfCore
{
    [AutoOptions("Shashlik.EfCore")]
    public class EfCoreOptions
    {
        public bool Enable { get; set; } = true;
    }
}