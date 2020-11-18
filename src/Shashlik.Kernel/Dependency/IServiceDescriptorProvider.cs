using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// ServiceTypeDescriptor provider
    /// </summary>
    public interface IServiceDescriptorProvider
    {
        IEnumerable<ShashlikServiceDescriptor> GetDescriptor(TypeInfo type);
    }
}