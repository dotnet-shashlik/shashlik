using System;

namespace Shashlik.Kernel.Assembler
{
    public interface IAssemblerExecutor<out T> where T : IAssembler
    {
        void Execute(IServiceProvider serviceProvider, Action<T> executor);
    }
}