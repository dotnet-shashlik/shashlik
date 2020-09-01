using System;

namespace Shashlik.Kernel
{
    public class KernelExceptionInitException : Exception
    {
        public KernelExceptionInitException(string message) : base(message)
        {
        }

        public KernelExceptionInitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}