using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.GeoCoder
{
    /// <summary>
    /// 地理位置区域
    /// </summary>
    public class GeoRectangle
    {
        /// <summary>
        /// 左下角纬度
        /// </summary>
        public decimal MinLat { get; set; }

        /// <summary>
        /// 左下角经度
        /// </summary>
        public decimal MinLng { get; set; }

        /// <summary>
        /// 右上角纬度
        /// </summary>
        public decimal MaxLat { get; set; }

        /// <summary>
        /// 右上角经度
        /// </summary>
        public decimal MaxLng { get; set; }
    }
}
