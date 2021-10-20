using System;

namespace Shashlik.Kernel.Assembler
{
    public class DefaultAssemblerExecutor<T> : IAssemblerExecutor<T> where T : IAssembler
    {
        private readonly IAssemblerProvider<T> _assemblerProvider;
        private readonly IKernelServices _kernelServices;

        public DefaultAssemblerExecutor(IAssemblerProvider<T> assemblerProvider, IKernelServices kernelServices)
        {
            _assemblerProvider = assemblerProvider;
            _kernelServices = kernelServices;
        }

        public void Execute(IServiceProvider serviceProvider, Action<T> executor)
        {
            var assemblerDescriptors = _assemblerProvider.Load(_kernelServices, serviceProvider);
            _assemblerProvider.Execute(assemblerDescriptors, r => executor(r.ServiceInstance));
        }
    }
}