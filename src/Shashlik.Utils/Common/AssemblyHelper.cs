using Guc.Utils.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyModel;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Guc.Utils.Common
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 获取引用了<paramref name="assembly"/>程序集的所有的程序集
        /// </summary>
        /// <param name="assembly">引用这个程序集</param>
        /// <param name="dependencyContext">依赖上下文,null则使用默认</param>
        /// <returns></returns>
        public static List<Assembly> GetReferredAssemblies(Assembly assembly, DependencyContext dependencyContext = null)
        {
            var allLib = (dependencyContext ?? DependencyContext.Default).RuntimeLibraries.OrderBy(r => r.Name).ToList();
            var name = assembly.GetName().Name;

            HashSet<string> excludes = new HashSet<string>();
            Dictionary<string, HashSet<string>> allDependencies = new Dictionary<string, HashSet<string>>();
            foreach (var item in allLib)
            {
                allDependencies.Add(item.Name, new HashSet<string>());
                loadAllDependency(allLib, item.Name, new HashSet<string>(), item, allDependencies);
            }

            return allDependencies.Where(r => r.Value.Contains(name)).Select(r =>
                   {
                       try
                       {
                           return Assembly.Load(r.Key);
                       }
                       catch
                       {
                           return null;
                       }
                   })
                .Where(r => r != null)
                .ToList();
        }

        /// <summary>
        /// 获取引用了<typeparamref name="TType"/>的程序集
        /// </summary>
        /// <param name="dependencyContext">依赖上下文,null则使用默认</param>
        /// <returns></returns>
        public static List<Assembly> GetReferredAssemblies<TType>(DependencyContext dependencyContext = null)
        {
            return GetReferredAssemblies(typeof(TType).Assembly, dependencyContext);
        }

        /// <summary>
        /// 获取引用了<typeparamref name="TType"/>的程序集
        /// </summary>
        /// <param name="dependencyContext">依赖上下文,null则使用默认</param>
        /// <returns></returns>
        public static List<Assembly> GetReferredAssemblies(Type type, DependencyContext dependencyContext = null)
        {
            return GetReferredAssemblies(type.Assembly, dependencyContext);
        }

        /// <summary>
        /// 获取所有的子类,不包括接口和抽象类
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<TypeInfo> GetChildFinalTypes(Type baseType, Assembly assembly)
        {
            return assembly.DefinedTypes.Where(r => !r.IsAbstract && r.IsClass && (r.IsChildTypeOf(baseType) || r.IsChildTypeOfGenericType(baseType))).ToList();
        }

        /// <summary>
        /// 获取所有的子类,不包括接口和抽象类
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static List<TypeInfo> GetChildFinalTypes(Type baseType, DependencyContext dependencyContext = null)
        {
            List<TypeInfo> types = new List<TypeInfo>();
            foreach (var item in GetReferredAssemblies(baseType, dependencyContext))
            {
                types.AddRange(GetChildFinalTypes(baseType, item));
            }

            return types;
        }

        /// <summary>
        /// 获取所有的子类,不包括接口和抽象类
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<TAttribute> GetTypes<TAttribute>(Assembly assembly)
            where TAttribute : Attribute
        {
            return assembly.DefinedTypes
                .Select(r => r.GetCustomAttribute<TAttribute>())
                .Where(r => r != null)
                .ToList();
        }

        /// <summary>
        /// 获取所有的子类,不包括接口和抽象类
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static List<TAttribute> GetChildFinalTypes<TAttribute>(DependencyContext dependencyContext = null)
            where TAttribute : Attribute
        {
            List<TAttribute> types = new List<TAttribute>();
            foreach (var item in GetReferredAssemblies<TAttribute>(dependencyContext))
            {
                types.AddRange(GetTypes<TAttribute>(item));
            }

            return types;
        }

        /// <summary>
        /// 加载所有的依赖
        /// </summary>
        /// <param name="allLibs">应用包含的所有程序集</param>
        /// <param name="key">当前计算的程序集name</param>
        /// <param name="handled">当前计算的程序集,已经处理过的依赖程序集名称</param>
        /// <param name="current">递归正在处理的程序集名称</param>
        /// <param name="allDependencies">所有的依赖数据</param>
        static void loadAllDependency(IEnumerable<RuntimeLibrary> allLibs, string key, HashSet<string> handled,
            RuntimeLibrary current, Dictionary<string, HashSet<string>> allDependencies)
        {
            if (current.Dependencies.IsNullOrEmpty())
                return;
            if (handled.Contains(current.Name))
                return;
            handled.Add(current.Name);

            foreach (var item in current.Dependencies)
            {
                allDependencies[key].Add(item.Name);
                var next = allLibs.FirstOrDefault(r => r.Name == item.Name);
                if (next == null || next.Dependencies.IsNullOrEmpty())
                    continue;
                loadAllDependency(allLibs, key, handled, next, allDependencies);
            }
        }
    }
}
