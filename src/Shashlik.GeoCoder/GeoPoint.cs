using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.GeoCoder
{

    /// <summary>
    /// 地理坐标
    /// </summary>
    public class GeoPoint
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Lng { get; set; }
    }
}
