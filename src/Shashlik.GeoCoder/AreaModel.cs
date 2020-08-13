using System.Collections.Generic;
using Guc.Utils.Extensions;

namespace Guc.GeoCoder
{
    /// <summary>
    /// 区域模型
    /// </summary>
    public class AreaModel
    {
        /// <summary>
        /// 省份 不用传
        /// </summary>      
        public string Province
        {
            get
            {
                if (ProvinceCode.IsNullOrWhiteSpace())
                    return null;
                else if (ProvinceCode == "000000")
                    return "其它省";
                else
                    return DefaultAreaService.Areas.ContainsKey(ProvinceCode) ? DefaultAreaService.Areas[ProvinceCode].Label : "";
            }
        }

        /// <summary>
        /// 省份编号
        /// </summary>     
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 城市 不用传
        /// </summary>    
        public string City
        {
            get
            {
                if (CityCode.IsNullOrWhiteSpace())
                    return null;
                else if (CityCode == "000000")
                    return "其它市";
                else
                {
                    if (DefaultAreaService.Areas.ContainsKey(ProvinceCode))
                    {
                        var p = DefaultAreaService.Areas[ProvinceCode];
                        if (p.Children.ContainsKey(CityCode))
                        {
                            return p.Children[CityCode].Label;
                        }
                    }

                    return "";
                }
            }
        }

        /// <summary>
        /// 城市编号
        /// </summary>     
        public string CityCode { get; set; }

        /// <summary>
        /// 区域 不用传
        /// </summary>     
        public string Area
        {
            get
            {
                if (AreaCode.IsNullOrWhiteSpace())
                    return null;
                else if (AreaCode == "000000")
                    return "其它区";
                else
                {
                    if (DefaultAreaService.Areas.ContainsKey(ProvinceCode))
                    {
                        var p = DefaultAreaService.Areas[ProvinceCode];
                        if (p.Children.ContainsKey(CityCode))
                        {
                            var c = p.Children[CityCode];
                            if (c.Children.ContainsKey(AreaCode))
                                return c.Children[AreaCode].Label;
                        }
                    }

                    return "";
                }
            }
        }

        /// <summary>
        /// 区域编号
        /// </summary>      
        public string AreaCode { get; set; }
    }

    /// <summary>
    /// 区域级别
    /// </summary>
    public enum AreaLevel
    {
        /// <summary>
        /// 省
        /// </summary>
        省 = 1,
        /// <summary>
        /// 市
        /// </summary>
        市 = 2,
        /// <summary>
        /// 区
        /// </summary>
        区 = 3
    }

    public class AreaItems
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public Dictionary<string, AreaItems> Children { get; set; }
    }
}
