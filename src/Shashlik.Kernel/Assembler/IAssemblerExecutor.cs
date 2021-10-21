using System;

namespace Shashlik.Kernel.Assembler
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IAssemblerExecutor<T> where T : IAssembler
    {
        void Execute(IServiceProvider serviceProvider, Action<T> executor);
    }
}