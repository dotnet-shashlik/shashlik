using System;

namespace Shashlik.EfCore.Transactional
{
    /// <summary>
    /// 指定默认的特性事务数据库上下文,一个系统只能设置一个
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DefaultTransactionalAttribute : Attribute
    {
    }
}