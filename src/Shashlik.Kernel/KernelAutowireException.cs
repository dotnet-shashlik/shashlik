using System;

namespace Shashlik.Kernel
{
    public class KernelAutowireException : Exception
    {
        public KernelAutowireException(string message) : base(message)
        {
        }

        public KernelAutowireException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}