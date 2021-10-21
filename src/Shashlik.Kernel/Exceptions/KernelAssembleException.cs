using System;

namespace Shashlik.Kernel.Exceptions
{
    /// <summary>
    /// 装载异常
    /// </summary>
    public class KernelAssembleException : Exception
    {
        public KernelAssembleException(string message) : base(message)
        {
        }

        public KernelAssembleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}