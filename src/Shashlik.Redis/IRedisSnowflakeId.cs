namespace Shashlik.Redis
{
    public interface IRedisSnowflakeId
    {
        /// <summary>
        /// 从redis中自动分配WorkerId、DataCenterId，一个应用中应该只调用一次，这里只是方法的定义和实现。已在<see cref="RedisSnowflakeId"/>中初始化，请勿在其他地方再调用。
        /// </summary>
        /// <returns></returns>
        (int workId, int dcId) GetId();

        /// <summary>
        /// 程序退出时清理id值
        /// </summary>
        void Clean();
    }
}