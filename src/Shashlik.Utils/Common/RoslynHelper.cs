using Guc.Utils.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Guc.Utils.Common
{
    /// <summary>
    /// C# 动态脚本编译
    /// </summary>
    public class RoslynHelper
    {
        /// <summary>
        /// 编译为类型
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type CompileToType(string content, string type)
        {
            var ass = CompileAssembly(type, content);
            return ass.ExportedTypes.First(r => r.Name == type);
        }

        /// <summary>
        /// 编辑为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T CompileToObject<T>(string content, string type)
        {
            var compliedType = CompileToType(content, type);
            if (compliedType == null) return default;
            var obj = Activator.CreateInstance(compliedType);
            if (typeof(T) == compliedType || typeof(T).IsAssignableFrom(compliedType)) return (T) obj;
            return default;
        }

        private static ConcurrentDictionary<string, Assembly> caches { get; } = new ConcurrentDictionary<string, Assembly>();

        /// <summary>
        /// 编译为程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Assembly CompileAssembly(string assemblyName, string content)
        {
            string key = content.Trim().Md532();
            if (caches.ContainsKey(key))
            {
                return caches[key];
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(content);

            // 指定编译选项。
            assemblyName = $"{assemblyName}.dynamic";
            var compilation = CSharpCompilation.Create
                (
                    assemblyName,
                    new[] { syntaxTree },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                )
                .AddReferences
                (
                    // 加入当前程序所有的引用
                    AppDomain.CurrentDomain.GetAssemblies().Select(x =>
                    {
                        try { return MetadataReference.CreateFromFile(x.Location); } catch { return null; }

                    }).Where(r => r != null)
                );

            // 编译到内存流中。
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());

                    caches.TryAdd(key, assembly);
                    return assembly;
                }
                throw new Exception("动态脚本编译错误:" + result.Diagnostics.First());
            }
        }
    }
}
