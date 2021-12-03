using System.IO;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Test;

namespace Shashlik.Captcha.Totp.Tests
{
    public class CaptchaTestConfigurationBuilder : ITestConfigurationBuilder
    {
        public void Build(IConfigurationBuilder builder)
        {
            var file = new FileInfo("./captcha.json").FullName;
            builder.AddJsonFile(file);
        }
    }
}