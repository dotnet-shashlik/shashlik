using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Automatic
{
    public class AutoDescriptor
    {
        /// <summary>
        /// 在我之后有哪些依赖
        /// </summary>
        public List<TypeInfo> DependsAfter { get; } = new List<TypeInfo>();
        /// <summary>
        /// 在我之前有哪些依赖
        /// </summary>
        public List<TypeInfo> DependsBefore { get; } = new List<TypeInfo>();

        /// <summary>
        /// 在哪些依赖之后
        /// </summary>
        public TypeInfo[] After { get; set; }

        /// <summary>
        /// 在哪些依赖之前
        /// </summary>
        public TypeInfo[] Before { get; set; }

        /// <summary>
        /// 自动装配服务类执行
        /// </summary>
        public TypeInfo ServiceType { get; set; }

        /// <summary>
        /// service实例
        /// </summary>
        public object ServiceInstance { get; set; }

        /// <summary>
        /// 注册状态,0:未注册,1:注册中,2:注册完成 
        /// </summary>
        public _Status Status { get; set; }

        public enum _Status : byte
        {
            /// <summary>
            /// 等待执行
            /// </summary>
            Waiting = 0,
            /// <summary>
            /// 挂起中,递归运行性,用于循环依赖检测
            /// </summary>
            Hangup = 1,
            /// <summary>
            /// 已执行完成
            /// </summary>
            Done = 2,
        }
    }
}
