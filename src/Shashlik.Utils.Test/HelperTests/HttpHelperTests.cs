// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Net;
// using System.Text;
// using Xunit;
// using Shouldly;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using RestSharp;
// using Shashlik.Utils.Helpers;
// using Xunit.Abstractions;
//
// namespace Shashlik.Utils.Test
// {
//     public class HttpHelperTests
//     {
//         private readonly ITestOutputHelper _testOutputHelper;
//
//         public HttpHelperTests(ITestOutputHelper testOutputHelper)
//         {
//             _testOutputHelper = testOutputHelper;
//         }
//
//         [Fact]
//         public async Task ErrorRequestTest()
//         {
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostForm("http://this_is_not_exist_host.com",
//                     new {name = "1"}, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostForm("http://this_is_not_exist_host.com",
//                     new List<KeyValuePair<string, string>>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostForm<string>("http://this_is_not_exist_host.com",
//                     new {name = "1"}, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostForm<string>("http://this_is_not_exist_host.com",
//                     new List<KeyValuePair<string, string>>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostJson("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostJson<string>("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostFiles("http://this_is_not_exist_host.com", new { },
//                     new List<UploadFileModel>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostFiles("http://this_is_not_exist_host.com",
//                     new List<KeyValuePair<string, string>>(),
//                     new List<UploadFileModel>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostFiles<string>("http://this_is_not_exist_host.com", new { },
//                     new List<UploadFileModel>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.PostFiles<string>("http://this_is_not_exist_host.com",
//                     new List<KeyValuePair<string, string>>(),
//                     new List<UploadFileModel>(), timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.Get<string>("http://this_is_not_exist_host.com", timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.GetBytes("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.GetStream("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.GetString("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.Get<EchoObject>("http://this_is_not_exist_host.com", new { }, timeout: 3);
//             });
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.GetString("http://this_is_not_exist_host.com", new { }, encoding: Encoding.UTF8,
//                     proxy: new WebProxy("127.0.0.1:8000"), timeout: 3);
//             });
//         }
//
//         [Fact]
//         public async Task PostContent_test()
//         {
//             var echo = await HttpHelper.PostJson<EchoObject>("https://postman-echo.com/post", new {data = "data"});
//             echo.ShouldNotBeNull();
//             echo.data.ShouldBeAssignableTo(typeof(IEnumerable));
//             //echo.data["data"].ToString().ShouldBe("data");
//             var json = await HttpHelper.PostJson("https://postman-echo.com/post", new {data = "data"});
//             var responseData = JsonConvert.DeserializeObject<JObject>(json);
//             responseData.ShouldNotBeNull();
//             responseData["data"].ShouldNotBeNull();
//             responseData["data"]["data"].ToString().ShouldBe("data");
//             var form = await HttpHelper.PostForm("https://postman-echo.com/post",
//                 new List<KeyValuePair<string, string>>()
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 });
//             var formResponseData = JsonConvert.DeserializeObject<JObject>(form);
//             formResponseData.ShouldNotBeNull();
//             formResponseData["form"].ShouldNotBeNull();
//             formResponseData["form"]["name"].Count().ShouldBe(2);
//             formResponseData["form"]["name"].Select(x => x.ToString()).ShouldContain("name1");
//             formResponseData["form"]["age"].ShouldBe("6");
//             form = await HttpHelper.PostForm("https://postman-echo.com/post", new {name = "name1", age = 6});
//             formResponseData = JsonConvert.DeserializeObject<JObject>(form);
//             formResponseData.ShouldNotBeNull();
//             formResponseData["form"].ShouldNotBeNull();
//             formResponseData["form"]["name"].ShouldBe("name1");
//             formResponseData["form"]["age"].ShouldBe("6");
//             var formEcho1 =
//                 await HttpHelper.PostForm<EchoObject>("https://postman-echo.com/post", new {name = "name1", age = 6});
//             formEcho1.ShouldNotBeNull();
//             formEcho1.form["name"].ShouldBe("name1");
//             formEcho1.form["age"].ShouldBe("6");
//             var formEcho2 = await HttpHelper.PostForm<EchoObject>("https://postman-echo.com/post",
//                 new List<KeyValuePair<string, string>>()
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 });
//             formEcho2.ShouldNotBeNull();
//             formEcho2.form.ShouldNotBeEmpty();
//             formEcho2.form["name"].ShouldBeAssignableTo(typeof(IEnumerable));
//             formEcho2.form["age"].ShouldBe("6");
//
//             var response = await HttpHelper.Post("https://postman-echo.com/post", "{}", "appliaction/json");
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//         }
//
//         [Fact]
//         public async Task PostFileTest()
//         {
//             var json = await HttpHelper.PostFiles("https://postman-echo.com/post", new {data = "data"},
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             var responseData = JsonConvert.DeserializeObject<JObject>(json);
//             responseData.ShouldNotBeNull();
//             responseData["files"].ShouldNotBeNull();
//             responseData["files"].Count().ShouldBe(1);
//             json = await HttpHelper.PostFiles("https://postman-echo.com/post", new List<KeyValuePair<string, string>>
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 },
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             responseData = JsonConvert.DeserializeObject<JObject>(json);
//             responseData.ShouldNotBeNull();
//             responseData["form"].ShouldNotBeNull();
//             responseData["files"].ShouldNotBeNull();
//             responseData["files"].Count().ShouldBe(1);
//             var echo = await HttpHelper.PostFiles<EchoObject>("https://postman-echo.com/post", new {data = "data"},
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             echo.ShouldNotBeNull();
//             echo.files.ShouldNotBeEmpty();
//
//             echo = await HttpHelper.PostFiles<EchoObject>("https://postman-echo.com/post",
//                 new List<KeyValuePair<string, string>>
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 },
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             echo.ShouldNotBeNull();
//             echo.files.ShouldNotBeEmpty();
//             echo.form.ShouldNotBeEmpty();
//         }
//
//         [Fact]
//         public async Task GetTest()
//         {
//             var json = await HttpHelper.GetString("https://postman-echo.com/get");
//             json.ShouldNotBeEmpty();
//             var stream = await HttpHelper.GetStream("https://postman-echo.com/get");
//             stream.ShouldNotBeNull();
//             var bytes = await HttpHelper.GetBytes("https://postman-echo.com/get");
//             bytes.ShouldNotBeEmpty();
//             var echo = await HttpHelper.Get<EchoObject>("https://postman-echo.com/get", new {query1 = "query1"});
//             echo.ShouldNotBeNull();
//             echo.args.ShouldNotBeEmpty();
//             echo.args["query1"].ShouldBe("query1");
//
//             json = await HttpHelper.GetString("https://postman-echo.com/get",
//                 headers: new Dictionary<string, string> {{"X-Test", "test"}},
//                 cookies: new Dictionary<string, string> {{"token", "token"}});
//             json.ShouldNotBeEmpty();
//         }
//
//         [Fact]
//         public async Task InvokeTest()
//         {
//             var json = await HttpHelper.DoRequest(Method.PUT, "https://postman-echo.com/put", "{}", "application/json",
//                 new List<KeyValuePair<string, string>>
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 });
//             json.ShouldNotBeNull();
//             json.IsSuccessful.ShouldBe(true);
//         }
//
//         [Fact]
//         public async Task OriginResponseTest()
//         {
//             var response = await HttpHelper.GetForOriginResponse("https://postman-echo.com/get", new {name = "name"});
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//
//             response = await HttpHelper.PostJsonForOriginResponse<EchoObject>("https://postman-echo.com/post",
//                 new {name = "name"});
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//             response = await HttpHelper.PostFormForOriginResponse("https://postman-echo.com/post", new {name = "name"});
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//             response = await HttpHelper.PostFormForOriginResponse("https://postman-echo.com/post",
//                 new List<KeyValuePair<string, string>>
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 });
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//             response = await HttpHelper.PostFilesForOriginResponse("https://postman-echo.com/post",
//                 new {name = "name"},
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//             response = await HttpHelper.PostFilesForOriginResponse("https://postman-echo.com/post",
//                 new List<KeyValuePair<string, string>>
//                 {
//                     new KeyValuePair<string, string>("name", "name1"),
//                     new KeyValuePair<string, string>("name", "name2"),
//                     new KeyValuePair<string, string>("age", "6"),
//                 },
//                 new List<UploadFileModel>()
//                 {
//                     new UploadFileModel()
//                     {
//                         FileBytes = await File.ReadAllBytesAsync("HelperTests/test_template.xlsx"),
//                         Name = "test_template.xlsx",
//                         FileName = "test_template.xlsx"
//                     }
//                 });
//             response.ShouldNotBeNull();
//             response.IsSuccessful.ShouldBe(true);
//         }
//
//         [Fact]
//         public async Task TimeoutTest()
//         {
//             await Should.ThrowAsync<Exception>(async () =>
//             {
//                 await HttpHelper.GetString("https://deelay.me/20000/http://www.baidu.com", timeout: 2);
//             });
//             await HttpHelper.GetForOriginResponse("https://deelay.me/5000/http://www.baidu.com", timeout: 6);
//         }
//     }
//
//     public class EchoObject
//     {
//         public Dictionary<string, string> args { get; set; }
//
//         public object data { get; set; }
//         public Dictionary<string, object> files { get; set; }
//         public Dictionary<string, object> form { get; set; }
//         public Dictionary<string, string> headers { get; set; }
//
//         public string url { get; set; }
//     }
// }