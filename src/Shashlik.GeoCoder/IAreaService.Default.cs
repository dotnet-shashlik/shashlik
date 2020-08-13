using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using Guc.Utils.Extensions;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json.Linq;
using Guc.Utils.Common;

namespace Guc.GeoCoder
{
    class DefaultAreaService : IAreaService, Guc.Kernel.Dependency.ISingleton
    {
        private static decimal _latitudeDistanceInKilometersDivisor = 111.045M;

        IOptions<GeoCoderOptions> GeoCoderOptions { get; }
        ILogger<DefaultAreaService> logger { get; }

        private static Lazy<IReadOnlyDictionary<string, AreaItems>> _areas = new Lazy<IReadOnlyDictionary<string, AreaItems>>(() => LoadAreas());
        internal static IReadOnlyDictionary<string, AreaItems> Areas => _areas.Value;

        /// <summary>
        /// 根节点 code
        /// </summary>
        public const string RootCode = "100000";

        public DefaultAreaService(IOptions<GeoCoderOptions> geoCoderOptions, ILogger<DefaultAreaService> logger)
        {
            this.GeoCoderOptions = geoCoderOptions;
            this.logger = logger;
        }

        public bool IsValid(AreaModel areaModel, AreaLevel level, out string errMsg)
        {
            errMsg = "";

            AreaItems p = null;
            if (level >= AreaLevel.省)
            {
                if (!Areas.ContainsKey(areaModel.ProvinceCode))
                {
                    errMsg = "请选择省";
                    return false;
                }
                p = Areas[areaModel.ProvinceCode];
            }

            AreaItems c = null;
            if (level >= AreaLevel.市)
            {
                if (areaModel.CityCode.IsNullOrWhiteSpace() || !p.Children.ContainsKey(areaModel.CityCode))
                {
                    errMsg = "请选择市";
                    return false;
                }
                c = p.Children[areaModel.CityCode];
            }

            if (level >= AreaLevel.区)
            {
                if (areaModel.AreaCode.IsNullOrWhiteSpace() || !c.Children.ContainsKey(areaModel.AreaCode))
                {
                    errMsg = "请选择区";
                    return false;
                }
            }

            return true;
        }

        public IDictionary<string, AreaItems> All()
        {
            return Areas.ToDictionary(r => r.Key, r => r.Value);
        }

        public async Task<AreaModel> ConvertToAreaModel(string address)
        {
            string res = null;
            try
            {
                res = await HttpHelper.GetString($"https://restapi.amap.com/v3/geocode/geo?address={address}&key={GeoCoderOptions.Value.Key}");
                var resObj = JsonConvert.DeserializeObject<JObject>(res);

                if (resObj["status"].Value<string>() != "1")
                    return null;

                //string code = resObj.geocodes[0].adcode;
                string code = (resObj["geocodes"] as JArray)[0]["adcode"].Value<string>();

                foreach (var p in DefaultAreaService.Areas)
                {
                    foreach (var c in p.Value.Children)
                    {
                        if (c.Value.Children.ContainsKey(code))
                        {
                            return new AreaModel
                            {
                                AreaCode = code,
                                CityCode = c.Key,
                                ProvinceCode = p.Key
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"请求高德地理数据接口错误,res:{res}");
                return null;
            }
        }

        public async Task<AreaModel> ConvertToAreaModel(decimal lat, decimal lng)
        {
            string res = null;
            try
            {
                res = await HttpHelper.GetString($"https://restapi.amap.com/v3/geocode/regeo?location={lng},{lat}&key={GeoCoderOptions.Value.Key}&extensions=base");
                var resObj = JsonConvert.DeserializeObject<JObject>(res);

                if (resObj["status"].Value<string>() != "1")
                    return null;

                //string code = resObj.geocodes[0].adcode;
                string code = resObj["regeocode"]["addressComponent"]["adcode"].Value<string>();

                foreach (var p in DefaultAreaService.Areas)
                {
                    foreach (var c in p.Value.Children)
                    {
                        if (c.Value.Children.ContainsKey(code))
                        {
                            return new AreaModel
                            {
                                AreaCode = code,
                                CityCode = c.Key,
                                ProvinceCode = p.Key
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"请求高德地理数据接口错误,res:{res}");
                return null;
            }
        }

        public AreaModel Get(string areaCode)
        {
            if (!computedDic.IsNullOrEmpty())
                return computedDic.GetOrDefault(areaCode);

            foreach (var province in Areas)
            {
                foreach (var city in province.Value.Children)
                {
                    foreach (var area in city.Value.Children)
                    {
                        computedDic.Add(area.Key, new AreaModel
                        {
                            AreaCode = area.Key,
                            CityCode = city.Key,
                            ProvinceCode = province.Key
                        });
                    }
                }
            }

            return computedDic.GetOrDefault(areaCode);
        }

        /// <summary>
        /// 获取与地理坐标对应距离的正方形区域,最大180,-180,90,-90
        /// </summary>
        /// <see cref="https://github.com/scottschluer/geolocation/blob/master/Geolocation/CoordinateBoundaries.cs"/>
        /// <param name="center"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public GeoRectangle GetRectangle(GeoPoint center, decimal distance)
        {
            var rectangle = new GeoRectangle();

            var latitudeConversionFactor = distance / _latitudeDistanceInKilometersDivisor;
            var longitudeConversionFactor = distance / _latitudeDistanceInKilometersDivisor / Convert.ToDecimal(Math.Abs(Math.Cos(Convert.ToDouble(center.Lat) * (Math.PI / 180))));

            rectangle.MinLat = center.Lat - latitudeConversionFactor;
            rectangle.MaxLat = center.Lat + latitudeConversionFactor;

            rectangle.MinLng = center.Lng - longitudeConversionFactor;
            rectangle.MaxLng = center.Lng + longitudeConversionFactor;

            // Adjust for passing over coordinate boundaries
            if (rectangle.MinLat < -90) rectangle.MinLat = -90;
            if (rectangle.MaxLat > 90) rectangle.MaxLat = 90;

            if (rectangle.MinLng < -180) rectangle.MinLng = -180;
            if (rectangle.MaxLng > 180) rectangle.MaxLng = 180;

            return rectangle;
        }

        public string GeoHash(GeoPoint point, int length = 8)
        {
            return NGeoHash.GeoHash.Encode(Convert.ToDouble(point.Lat), Convert.ToDouble(point.Lng), length);
        }

        #region private

        static IReadOnlyDictionary<string, AreaItems> LoadAreas()
        {
            GeoCoderOptions geoCoderOptions = Guc.Kernel.KernelServiceProvider.ServiceProvider.GetService<IOptions<GeoCoderOptions>>().Value;
            var all = geoCoderOptions.Areas;
            Dictionary<string, AreaItems> result = new Dictionary<string, AreaItems>();
            var provinceList = all[RootCode];

            foreach (var provinceItem in provinceList)
            {
                AreaItems province = new AreaItems
                {
                    Value = provinceItem.Key,
                    Label = provinceItem.Value,
                    Children = new Dictionary<string, AreaItems>()
                };

                if (!all.ContainsKey(provinceItem.Key))
                    continue;
                var cities = all[provinceItem.Key];
                foreach (var cityItem in cities)
                {
                    AreaItems city = new AreaItems
                    {
                        Value = cityItem.Key,
                        Label = cityItem.Value,
                        Children = new Dictionary<string, AreaItems>()
                    };

                    if (!all.ContainsKey(cityItem.Key))
                        continue;

                    var areas = all[cityItem.Key];
                    foreach (var areaItem in areas)
                    {
                        AreaItems area = new AreaItems
                        {
                            Value = areaItem.Key,
                            Label = areaItem.Value,
                        };
                        city.Children.Add(areaItem.Key, area);
                    }
                    province.Children.Add(cityItem.Key, city);
                }
                result.Add(provinceItem.Key, province);
            }

            return result;
        }

        static Dictionary<string, AreaModel> computedDic = new Dictionary<string, AreaModel>();


        #endregion

    }
}
