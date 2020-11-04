﻿using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Shashlik.Cap
{
    /// <summary>
    /// 事件订阅处理
    /// </summary>
    public interface IEventHandler<in TEvent> : ICapSubscribe, Kernel.Dependency.ITransient
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