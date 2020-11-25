using System.ComponentModel.DataAnnotations;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Options
{
    [AutoOptions("TestOptions5")]
    public class TestOptions5
    {
        [Required] [StringLength(1)] public string Name { get; set; }
    }
}