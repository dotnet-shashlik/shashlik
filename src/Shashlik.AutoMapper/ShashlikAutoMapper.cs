namespace Shashlik.AutoMapper
{
    public static class ShashlikAutoMapper
    {
        /// <summary>
        /// AutoMapper实例,注册后才有值
        /// </summary>
        public static global::AutoMapper.IMapper? Instance { get; internal set; }
    }
}