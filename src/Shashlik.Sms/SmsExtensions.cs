using System.Threading.Tasks;

namespace Shashlik.Sms
{
    public static class SmsExtensions
    {
        /// <summary>
        /// 快捷发送验证码方法,约定验证码subject为"Captcha"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="phone">手机号码</param>
        /// <param name="args">短信参数</param>
        /// <returns></returns>
        public static async Task<string> SendCaptchaAsync(this ISmsSender sender, string phone, params string[] args)
        {
            return await sender.SendWithLimitCheckAsync(phone, SmsConstants.SubjectCaptcha, args);
        }
    }
}