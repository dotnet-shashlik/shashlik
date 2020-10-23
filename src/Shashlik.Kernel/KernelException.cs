using System;

namespace Shashlik.Kernel
{
    public class KernelException : Exception
    {
        public KernelException(string message) : base(message)
        {
        }

        public KernelException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}