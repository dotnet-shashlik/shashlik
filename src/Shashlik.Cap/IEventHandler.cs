using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Cap
{
    /// <summary>
    /// 事件订阅处理
    /// </summary>
    [Transient(typeof(IEventHandler<>), RequireRegistryInheritedChain = true)]
    public interface IEventHandler<in TEvent> : ICapSubscribe
        where TEvent : class, IEvent
    {
        /// <summary>
        /// 执行处理逻辑
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Execute(TEvent @event);
    }
}