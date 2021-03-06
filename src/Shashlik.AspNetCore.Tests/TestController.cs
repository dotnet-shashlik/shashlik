﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shashlik.AspNetCore.Filters;
using Shashlik.Response;

namespace Shashlik.AspNetCore.Tests
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("api1")]
        [ResponseWrapper]
        [AllowAnonymous]
        public string Test1()
        {
            return "api1";
        }

        [HttpPost("api2")]
        [ExceptionWrapper]
        [AllowAnonymous]
        public string Test2()
        {
            throw new Exception("api2_ex");
        }

        [HttpPost("api3")]
        [ResponseWrapper]
        [NoResponseWrapper]
        [AllowAnonymous]
        public string Test3()
        {
            return "api3";
        }

        [HttpPost("api4")]
        [ExceptionWrapper(FindDepthResponseException = 1)]
        //[NoExceptionWrapper] 这个无法测试
        [AllowAnonymous]
        public string Test4()
        {
            throw new Exception("api4_ex");
        }

        [HttpPost("api5")]
        [ResponseWrapper(ModelError2HttpOk = false, ResponseAllModelError = true)]
        [AllowAnonymous]
        public string Test5(Input input)
        {
            return "api5";
        }

        [HttpPost("api6")]
        [ExceptionWrapper(UseResponseExceptionToHttpCode = true)]
        [AllowAnonymous]
        public string Test6()
        {
            throw ResponseException.ArgError();
        }

        [HttpPost("api7")]
        [ExceptionWrapper(UseResponseExceptionToHttpCode = false)]
        [AllowAnonymous]
        public string Test7()
        {
            throw ResponseException.ArgError();
        }
    }

    public class Input
    {
        [Required(ErrorMessage = "name_null")] public string Name { get; set; }

        [Range(1, 18, ErrorMessage = "age:1-18")]
        public int Age { get; set; }
    }
}