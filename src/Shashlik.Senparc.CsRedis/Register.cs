/*----------------------------------------------------------------
    Copyright (C) 2020 Senparc

    文件名：Register.cs
    文件功能描述：Senparc.CO2NET.Cache.Redis 快捷注册流程


    创建标识：Senparc - 20180222

    修改标识：Senparc - 20180606
    修改描述：缓存工厂重命名为 ContainerCacheStrategyFactory

    修改标识：Senparc - 20180802
    修改描述：v3.1.0 1、Register.RegisterCacheRedis 标记为过期
                     2、新增 Register.SetConfigurationOption() 方法
                     3、新增 Register.UseKeyValueRedisNow() 方法
                     4、新增 Register.UseHashRedisNow() 方法

----------------------------------------------------------------*/

//using Senparc.CO2NET.Cache;
using CSRedis;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.RegisterServices;
using System;

namespace Guc.Senparc.CsRedis
{
    /// <summary>
    /// Redis 注册
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// 注册键值对 redis缓存策略
        /// </summary>
        /// <param name="csRedisClient"></param>
        public static void UseKeyValueRedisNow(CSRedisClient csRedisClient)
        {
            new RedisObjectCacheStrategy(csRedisClient);
            CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//键值Redis
            _ = RedisContainerCacheStrategy.Instance;
        }
    }
}
