using System;

namespace Shashlik.Kernel
{
    public class KernelServiceException : Exception
    {
        public KernelServiceException(string message) : base(message)
        {
        }

        public KernelServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}