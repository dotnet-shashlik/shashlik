namespace Shashlik.Kernel
{
    /// <summary>
    /// 自动装配基类,装配器一般只执行一次,建议配置为Transient<para></para>
    /// 注意: **服务装配类所有的条件特性<see cref="Shashlik.Kernel.Attributes.ConditionBaseAttribute"/>都不可用** 
    /// </summary>
    public interface IAssembler
    {
    }
}