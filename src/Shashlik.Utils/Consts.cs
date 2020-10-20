using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shashlik.Utils
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Consts
    {
        /// <summary>
        /// 常用正则
        /// </summary>
        public class Regexs
        {
            /// <summary>
            /// 邮件地址
            /// </summary>
            [EmailAddress] public const string Email = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            /// <summary>
            /// 国内手机号码
            /// </summary>
            public const string MobilePhoneNumber = @"^[1][0-9]{10}$";
            /// <summary>
            /// 国内身份证18位
            /// </summary>
            public const string IdCard = @"(^\d{18}$)|(^\d{17}(\d|X|x)$)";
            /// <summary>
            /// 数字
            /// </summary>
            public const string Number = @"^[0-9]+$";
            /// <summary>
            /// 英文和数字
            /// </summary>
            public const string LetterOrNumber = @"^[A-Za-z0-9]+$";
            /// <summary>
            /// 英文和数字 下划线
            /// </summary>
            public const string LetterOrNumberOrUnderline = @"^[0-9a-zA-Z_]+$";
            /// <summary>
            /// 全部汉字
            /// </summary>
            public const string AllChineseChar = @"^[\u4e00-\u9fa5]+$";
            /// <summary>
            /// 包含汉字
            /// </summary>
            public const string ContainsChineseChar = @"[\u4e00-\u9fa5]+";
            /// <summary>
            /// URL,必须包含http/https前缀
            /// </summary>
            public const string Url = @"^((https|http)?:\/\/)[^\s]+";
            /// <summary>
            /// ipv4地址
            /// </summary>
            public const string Ipv4 = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        }
    }
}
