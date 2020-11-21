// using System;
// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Shashlik.AspNetCore.Filters;
// using Shashlik.Response;
//
// namespace Shashlik.AspNetCore.Tests
// {
//     [Produces("application/json")]
//     [Route("[controller]")]
//     [ApiController]
//     public class TestController : ControllerBase
//     {
//         [HttpPost("api1")]
//         [ResponseWrapper]
//         [AllowAnonymous]
//         public string Test1()
//         {
//             return "api1";
//         }
//
//         [HttpPost("api2")]
//         [ExceptionWrapper]
//         [AllowAnonymous]
//         public string Test2()
//         {
//             throw new Exception("api2_ex");
//             return "api2";
//         }
//
//         [HttpPost("api2")]
//         [ResponseWrapper]
//         [NoResponseWrapper]
//         [AllowAnonymous]
//         public string Test3()
//         {
//             return "api3";
//         }
//
//         [HttpPost("api4")]
//         [ExceptionWrapper]
//         [NoExceptionWrapper]
//         [AllowAnonymous]
//         public string Test4()
//         {
//             throw new Exception("api4_ex");
//             return "api4";
//         }
//
//         [HttpPost("api5")]
//         [ResponseWrapper(ModelError2HttpOk = false, ResponseAllModelError = true)]
//         [AllowAnonymous]
//         public string Test5(Input input)
//         {
//             return "api5";
//         }
//
//         [HttpPost("api6")]
//         [ExceptionWrapper(UseResponseExceptionToHttpCode = true)]
//         [AllowAnonymous]
//         public string Test6()
//         {
//             throw ResponseException.ArgError();
//             return "api6";
//         }
//
//         [HttpPost("api7")]
//         [ExceptionWrapper(UseResponseExceptionToHttpCode = false)]
//         [AllowAnonymous]
//         public string Test7()
//         {
//             throw ResponseException.ArgError();
//             return "api7";
//         }
//     }
//
//     public class Input
//     {
//         [Required(ErrorMessage = "name_null")] public string Name { get; set; }
//
//         [Range(1, 18, ErrorMessage = "age:1-18")]
//         public int Age { get; set; }
//     }
// }