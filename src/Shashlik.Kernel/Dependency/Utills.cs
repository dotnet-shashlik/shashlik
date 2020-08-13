using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Kernel.Dependency
{
    static class Utils
    {

        /// <summary>
        /// 泛型参数是否匹配
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static bool GenericArgumentsIsMatch(Type[] arg1, Type[] arg2)
        {
            if (arg1 == null || arg1.Length == 0)
                return false;

            if (arg2 == null || arg2.Length == 0)
                return false;

            if (arg1.Length != arg2.Length)
                return false;

            for (int i = 0; i < arg1.Length; i++)
            {
                if (arg1[i] != arg2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 是否为约定的接口类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConvectionInterfaceType(Type type)
        {
            if (type == typeof(ITransient) ||
                type == typeof(ISingleton) ||
                type == typeof(IScoped))
                return true;
            return false;
        }
    }
}
