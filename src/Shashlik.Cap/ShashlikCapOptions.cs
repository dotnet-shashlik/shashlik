using DotNetCore.CAP;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Cap
{
    [AutoOptions("Shashlik.Cap")]
    public class ShashlikCapOptions : CapOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}