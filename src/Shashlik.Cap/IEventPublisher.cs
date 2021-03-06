﻿using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Shashlik.Cap
{
    /// <summary>
    /// 事件发布,事件类型的TypeName将作为事件名称Name
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// cap事件发布对象
        /// </summary>
        ICapPublisher CapPublisher { get; }

        /// <summary>
        /// 事件发布,事件类型的TypeName将作为事件名称Name
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventModel">事件模型</param>
        /// <param name="callbackName"></param>
        void Publish<T>(T eventModel, string? callbackName = null) where T : class, IEvent;

        /// <summary>
        /// 事件发布,事件类型的TypeName将作为事件名称Name
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventModel">事件模型</param>
        /// <param name="callbackName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T eventModel, string? callbackName = null, CancellationToken cancellationToken = default)
            where T : class, IEvent;
    }
}