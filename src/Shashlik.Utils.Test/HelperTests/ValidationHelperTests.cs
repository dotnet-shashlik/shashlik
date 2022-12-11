using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shashlik.Utils.Test.HelperTests
{
    public class ValidationHelperTests
    {
        [Fact]
        public void ValidationShouldBeOkTest()
        {
            var obj = new ValidationClass1
            {
                Name = "张三",
                Names1 = new string[] { "1", "2" },
                Names2 = new string[] { "1", "2" },
                JObject = new { Age = 1 }.ToJson().DeserializeJson<JObject>(),
                JsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new { Age = 1 })),
                Class2 = new ValidationClass2
                {
                    Name = "李四",
                    Names1 = new string[] { "1", "2" },
                    Names2 = new string[] { "1", "2" },
                    JObject = new { Age = 1 }.ToJson().DeserializeJson<JObject>(),
                    JsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new { Age = 1 })),
                }
            };

            obj.TryValidateObjectRecursion(out var _).ShouldBeTrue();
        }

        [Fact]
        public void ValidationErrorCountShouldBe4Test()
        {
            var obj = new ValidationClass1
            {
            };

            obj.TryValidateObjectRecursion(out var _).ShouldBeFalse();
        }

        [Fact]
        public void ValidationChildPropertyShouldBeFailTest()
        {
            var obj = new ValidationClass1
            {
                Name = "张三",
                Names1 = new string[] { "1", "2" },
                Names2 = new string[] { "1", "2" },
                JObject = new { Age = 1 }.ToJson().DeserializeJson<JObject>(),
                JsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new { Age = 1 })),
                Class2 = new ValidationClass2
                {
                }
            };


            obj.TryValidateObjectRecursion( out var _).ShouldBeFalse();
            //ValidationHelper.Validate(obj).Count.ShouldBe(3);
        }


        public class ValidationClass1
        {
            public string this[string index] => $"{Name}.{index}";

            [Required] [StringLength(10)] public string Name { get; set; }

            [MaxLength(2)] public string[] Names1 { get; set; }
            [Required] public IEnumerable<string> Names2 { get; set; }

            [Required] public JObject JObject { get; set; }

            public JsonElement JsonElement { get; set; }

            [Required] public ValidationClass2 Class2 { get; set; }
        }

        public class ValidationClass2
        {
            [Required] [StringLength(10)] public string Name { get; set; }

            [MaxLength(2)] public string[] Names1 { get; set; }
            [Required] public IEnumerable<string> Names2 { get; set; }

            [Required] public JObject JObject { get; set; }

            public JsonElement JsonElement { get; set; }
        }
    }
}