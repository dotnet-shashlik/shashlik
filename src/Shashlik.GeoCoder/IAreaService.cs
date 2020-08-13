using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using Guc.Utils.Extensions;
using System.Threading.Tasks;

namespace Guc.GeoCoder
{
    public interface IAreaService
    {
        /// <summary>
        /// 验证区域数据
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsValid(AreaModel areaModel, AreaLevel level, out string errMsg);

        /// <summary>
        /// 获取区域模型
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        AreaModel Get(string areaCode);

        /// <summary>
        /// 获取所有的定义的区域数据
        /// </summary>
        /// <returns></returns>
        IDictionary<string, AreaItems> All();

        /// <summary>
        /// 将地址信息转换为区域模型
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<AreaModel> ConvertToAreaModel(string address);

        /// <summary>
        /// 将经纬度转换为区域模型
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        Task<AreaModel> ConvertToAreaModel(decimal lat, decimal lng);

        /// <summary>
        /// 获取与地理坐标对应距离的正方形区域,最大180,-180,90,-90
        /// </summary>
        /// <see cref="https://github.com/scottschluer/geolocation/blob/master/Geolocation/CoordinateBoundaries.cs"/>
        /// <param name="center"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        GeoRectangle GetRectangle(GeoPoint center, decimal distance);

        /// <summary>
        /// 把GPS坐标转换成GeoHash
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Geohash"/>
        /// <param name="point"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        string GeoHash(GeoPoint point, int length = 8);
    }
}
