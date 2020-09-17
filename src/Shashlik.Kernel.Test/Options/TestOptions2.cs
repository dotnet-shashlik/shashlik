using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Kernel.Test.Options
{
    [AutoOptions("TestOptions2")]
    public class TestOptions2
    {
        public bool Enable { get; set; }
    }
}