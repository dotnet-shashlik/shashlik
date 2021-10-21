using System.ComponentModel.DataAnnotations;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Options
{
    [AutoOptions("TestOptions4")]
    public class TestOptions4
    {
        [Required]
        public string Name { get; set; }
    }
}