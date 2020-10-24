using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using DotNetCore.CAP;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Options;
using DotNetCore.CAP.Internal;

#pragma warning disable 8618

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Shashlik.EventBus
{
    internal class EventBusConsumerServiceSelector : IConsumerServiceSelector
    {
        private readonly CapOptions _capOptions;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// since this class be designed as a Singleton service,the following two list must be thread safe!!!
        /// </summary>
        private readonly ConcurrentDictionary<string, List<RegexExecuteDescriptor<ConsumerExecutorDescriptor>>>
            _asteriskList;

        private readonly ConcurrentDictionary<string, List<RegexExecuteDescriptor<ConsumerExecutorDescriptor>>>
            _poundList;

        /// <summary>
        /// Creates a new <see cref="EventBusConsumerServiceSelector" />.
        /// </summary>
        public EventBusConsumerServiceSelector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _capOptions = serviceProvider.GetService<IOptions<CapOptions>>().Value;

            _asteriskList =
                new ConcurrentDictionary<string, List<RegexExecuteDescriptor<ConsumerExecutorDescriptor>>>();
            _poundList = new ConcurrentDictionary<string, List<RegexExecuteDescriptor<ConsumerExecutorDescriptor>>>();
        }

        public IReadOnlyList<ConsumerExecutorDescriptor> SelectCandidates()
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();

            executorDescriptorList.AddRange(FindConsumersFromInterfaceTypes(_serviceProvider));
            return executorDescriptorList;
        }

        public ConsumerExecutorDescriptor SelectBestCandidate(string key,
            IReadOnlyList<ConsumerExecutorDescriptor> executeDescriptor)
        {
            var result = MatchUsingName(key, executeDescriptor);
            if (result != null)
            {
                return result;
            }

            //[*] match with regex, i.e.  foo.*.abc
            result = MatchAsteriskUsingRegex(key, executeDescriptor);
            if (result != null)
            {
                return result;
            }

            //[#] match regex, i.e. foo.#
            result = MatchPoundUsingRegex(key, executeDescriptor);
            return result;
        }

        private IEnumerable<ConsumerExecutorDescriptor> FindConsumersFromInterfaceTypes(
            IServiceProvider provider)
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();

            using var scoped = provider.CreateScope();
            var scopedProvider = scoped.ServiceProvider;
            var consumerServices = scopedProvider.GetServices<ICapSubscribe>();
            foreach (var service in consumerServices)
            {
                var typeInfo = service.GetType().GetTypeInfo();

                // 必须是非抽象类
                if (!typeInfo.IsClass || typeInfo.IsAbstract)
                    continue;

                if (!typeInfo.IsSubTypeOfGenericType(typeof(IEventHandler<>)))
                    continue;

                executorDescriptorList.AddRange(GetTopicAttributesDescription(typeInfo));
            }

            return executorDescriptorList;
        }

        (TypeInfo serviceType, TypeInfo eventType, string groupName, string eventName) GetEventHandlerInfo(
            TypeInfo type)
        {
            string groupName = $"{type.Name}.{_capOptions.Version}";
            var groupNameAttribute = type.GetCustomAttribute<NameAttribute>(true);
            if (groupNameAttribute != null)
                groupName = groupNameAttribute.Name;

            var types = new List<(TypeInfo serviceType, TypeInfo eventType, string groupName, string eventName)>();
            foreach (var item in type.ImplementedInterfaces)
            {
                var @interface = item.GetTypeInfo();
                if (!@interface.IsGenericType)
                    continue;
                if (@interface.GenericTypeArguments.Length != 1)
                    continue;

                var eventType = @interface.GenericTypeArguments[0].GetTypeInfo();
                if (!eventType.IsSubTypeOf<IEvent>())
                    continue;

                string eventName = $"{eventType.Name}.{_capOptions.Version}";
                var eventNameAttribute = type.GetCustomAttribute<NameAttribute>(true);
                if (eventNameAttribute != null)
                    eventName = eventNameAttribute.Name;

                types.Add((typeof(IEventHandler<>).MakeGenericType(eventType).GetTypeInfo(), eventType, groupName,
                    eventName));
            }

            if (types.Count == 0)
                throw new InvalidOperationException($"Can not find interface of IEventHandler<> on {type}.");
            if (types.Count > 1)
                throw new InvalidOperationException($"Find more interface of IEventHandler<> on {type}.");
            return types[0];
        }

        private IEnumerable<ConsumerExecutorDescriptor> GetTopicAttributesDescription(TypeInfo typeInfo)
        {
            var (serviceType, eventType, groupName, eventName) = GetEventHandlerInfo(typeInfo);

            List<ConsumerExecutorDescriptor> results = new List<ConsumerExecutorDescriptor>();

            var method = serviceType.GetMethod("Execute", new Type[] {eventType})!;

            results.Add(new ConsumerExecutorDescriptor
            {
                Attribute =
                    new CapSubscribeAttribute(eventName) {Group = groupName},
                ImplTypeInfo = typeInfo,
                MethodInfo = method,
                ServiceTypeInfo = serviceType,
                Parameters = method.GetParameters()
                    .Select(parameter => new ParameterDescriptor
                    {
                        Name = parameter.Name,
                        ParameterType = parameter.ParameterType,
                        IsFromCap = parameter.GetCustomAttributes(typeof(FromCapAttribute)).Any()
                    }).ToList()
            });

            return results;
        }

        private ConsumerExecutorDescriptor MatchUsingName(string key,
            IEnumerable<ConsumerExecutorDescriptor> executeDescriptor)
        {
            return executeDescriptor.FirstOrDefault(x => x.Attribute.Name == key);
        }

        private ConsumerExecutorDescriptor MatchAsteriskUsingRegex(string key,
            IReadOnlyList<ConsumerExecutorDescriptor> executeDescriptor)
        {
            var group = executeDescriptor.First().Attribute.Group;
            if (!_asteriskList.TryGetValue(group, out var tmpList))
            {
                tmpList = executeDescriptor.Where(x => x.Attribute.Name.IndexOf('*') >= 0)
                    .Select(x => new RegexExecuteDescriptor<ConsumerExecutorDescriptor>
                    {
                        Name = ("^" + x.Attribute.Name + "$").Replace("*", "[0-9_a-zA-Z]+").Replace(".", "\\."),
                        Descriptor = x
                    }).ToList();
                _asteriskList.TryAdd(group, tmpList);
            }

            foreach (var red in tmpList)
            {
                if (Regex.IsMatch(key, red.Name, RegexOptions.Singleline))
                {
                    return red.Descriptor;
                }
            }

            return null!;
        }

        private ConsumerExecutorDescriptor MatchPoundUsingRegex(string key,
            IReadOnlyList<ConsumerExecutorDescriptor> executeDescriptor)
        {
            var group = executeDescriptor.First().Attribute.Group;
            if (!_poundList.TryGetValue(group, out var tmpList))
            {
                tmpList = executeDescriptor
                    .Where(x => x.Attribute.Name.IndexOf('#') >= 0)
                    .Select(x => new RegexExecuteDescriptor<ConsumerExecutorDescriptor>
                    {
                        Name = ("^" + x.Attribute.Name + "$").Replace("#", "[0-9_a-zA-Z\\.]+"),
                        Descriptor = x
                    }).ToList();
                _poundList.TryAdd(group, tmpList);
            }

            foreach (var red in tmpList)
            {
                if (Regex.IsMatch(key, red.Name, RegexOptions.Singleline))
                {
                    return red.Descriptor;
                }
            }

            return null!;
        }


        private class RegexExecuteDescriptor<T>
        {
            public string Name { get; set; }

            public T Descriptor { get; set; }
        }
    }
}