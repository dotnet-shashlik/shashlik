using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Options
{
    [AutoOptions("TestOptions1")]
    public class TestOptions1
    {
        public bool Enable { get; set; }
    }
}