//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;
//using Shashlik.Utils.Common;
//using Shouldly;
//using System.Collections.Concurrent;
//using System.Linq;
//using Shashlik.Utils.Extensions;
//using System.ComponentModel.DataAnnotations;
//using Shashlik.Validation;
//using Newtonsoft.Json.Linq;
//using Microsoft.Extensions.DependencyInjection;
//using Shashlik.Kernel;
//using System.Reflection;
//using Shashlik.Utils.PatchUpdate;
//using Newtonsoft.Json;

//namespace Shashlik.Utils.Test
//{
//    public class ValidationTests
//    {
//        [Fact]
//        void isValid_test()
//        {
//            IServiceCollection services = new ServiceCollection();
//            services.AddShashlik()
//                .AddValidation();
//            var serviceProvider = services.BuildServiceProvider();
//            serviceProvider.UseShashlik()
//                .UseValidation();

//            {
//                PatchUpdateTest model = new PatchUpdateTest();

//                var input = new
//                {
//                    areaCode = "150404",
//                    cityCode = "150400",
//                    provinceCode = "150000",
//                };
//                var jObject = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(input));

//                model.PatchUpdateBase = new UserUpdateInput(jObject);
//                var res = model.IsValid();
//            }

//            {
//                var model = new TestA();
//                var res = model.IsValid();
//                res.Success.ShouldBeFalse();

//                model.Name = "egerherh";
//                model.Age = 15;
//                var res1 = model.IsValid();
//                res1.Success.ShouldBeTrue();

//                model.Relation = new TestA();
//                var res2 = model.IsValid();
//                res2.Success.ShouldBeFalse();

//                model.Relation.Age = 18;
//                model.Relation.Name = "222222";
//                var res3 = model.IsValid();
//                res3.Success.ShouldBeTrue();

//                model.Children = new List<TestA>() {
//                new TestA()
//            };
//                var res4 = model.IsValid();
//                res4.Success.ShouldBeFalse();

//                model.Children[0].Age = 18;
//                model.Children[0].Name = "222222";
//                var res5 = model.IsValid();
//                res5.Success.ShouldBeTrue();

//                var test = new ListStringLengthTest();
//                test.S = new List<string> { "123123123123" };
//                test.IsValid().Success.ShouldBeFalse();

//                var enumTest = new EnumTest();
//                enumTest.Value1 = EnumTest.T.A;
//                enumTest.Value2 = null;
//                enumTest.IsValid().Success.ShouldBeTrue();
//                enumTest.Value2 = EnumTest.T.A;
//                enumTest.IsValid().Success.ShouldBeTrue();
//            }

//            {
//                var model = new TestJToken();
//                var res = model.IsValid();
//                res.Success.ShouldBeFalse();
//                model.Name = "asdasd";
//                model.JObject = new JObject();
//                model.JObject.Add("test", new JValue(111));

//                model.Dic = new Dictionary<string, object> {
//                    { "test",111}

//                };
//                res = model.IsValid();
//                res.Success.ShouldBeTrue();
//            }
//        }

//        public class TestA
//        {
//            [Required(ErrorMessage = "Required")]
//            [MaxLength(18, ErrorMessage = "max 18")]
//            public string Name { get; set; }

//            [Range(1, 100, ErrorMessage = "age 1-100")]
//            public int Age { get; set; }

//            public List<TestA> Children { get; set; }

//            public TestA Relation { get; set; }
//        }

//        public class ListStringLengthTest
//        {
//            [ListStringLength(10)]
//            public List<string> S { get; set; }
//        }

//        public class EnumTest
//        {
//            [Enum]
//            public T Value1 { get; set; }

//            [Enum]
//            public T? Value2 { get; set; }
//            public enum T
//            {
//                A
//            }
//        }

//        public class TestJToken
//        {
//            [Required]
//            public string Name { get; set; }

//            public JObject JObject { get; set; }

//            public JArray JArray { get; set; }

//            public Dictionary<string, object> Dic { get; set; }
//        }

//        public class PatchUpdateTest
//        {
//            public PatchUpdateBase PatchUpdateBase { get; set; }
//        }

//        public class UserUpdateInput : PatchUpdateBase
//        {
//            public UserUpdateInput(JObject jObject) : base(jObject)
//            {
//            }

//            /// <summary>
//            /// 昵称
//            /// </summary>
//            [MaxLength(20, ErrorMessage = "昵称最多20个字符")]
//            [PRequired(ErrorMessage = "昵称不能为空")]
//            public string NickName { get; set; }

//            /// <summary>
//            /// 头像
//            /// </summary>
//            [PRequired(ErrorMessage = "头像不能为空")]
//            public string Avatar { get; set; }

//            /// <summary>
//            /// 生日
//            /// </summary>
//            public DateTime? Birthday { get; set; }

//            /// <summary>
//            /// 所在省份编号
//            /// </summary>     
//            public string ProvinceCode { get; set; }

//            /// <summary>
//            /// 所在城市编号
//            /// </summary>     
//            public string CityCode { get; set; }

//            /// <summary>
//            /// 所在区域编号
//            /// </summary>      
//            public string AreaCode { get; set; }

//        }
//    }
//}
