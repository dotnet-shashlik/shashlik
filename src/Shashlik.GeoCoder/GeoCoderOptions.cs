using System.Collections.Generic;

namespace Shashlik.GeoCoder
{
    public class GeoCoderOptions
    {
        /// <summary>
        /// 高德key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 行政区域数据
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Areas { get; set; }

    }
}
