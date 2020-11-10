using Shashlik.Kernel.Attributes;

namespace Shashlik.AutoMapper
{
    [AutoOptions("Shashlik.AutoMapper")]
    public class AutoMapperOptions
    {
        public bool Enable { get; set; } = true;
    }
}