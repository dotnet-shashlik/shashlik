using System.ComponentModel.DataAnnotations;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Options
{
    [AutoOptions("TestOptions4")]
    public class TestOptions4
    {
        [StringLength(1)]
        public string Name { get; set; }
    }
}