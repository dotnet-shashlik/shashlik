namespace Shashlik.EfCore
{
    /// <summary>
    /// ef core实体基础接口
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// 带泛型主键的ef core实体基础接口
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        TKey Id { get; set; }
    }
}