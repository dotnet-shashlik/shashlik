using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Guc.Kernel;
using Newtonsoft.Json;
using Guc.Utils.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Guc.Bank
{
    /// <summary>
    /// 银行服务
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IKernelBuilder AddBank(this IKernelBuilder kernelBuilder, IConfigurationSection configuration = null)
        {
            var services = kernelBuilder.Services;
            if (configuration != null)
                services.Configure<BankOptions>(configuration);
            else
            {
                services.Configure<BankOptions>(r =>
                {
                    var configurationOption = configuration?.Get<BankOptions>();

                    r.Datas = new List<BankModel>();
                    using (var stream = typeof(IBankService).Assembly.GetManifestResourceStream("Guc.Bank.data.txt"))
                    {
                        var lines = stream.ReadToString().Split(new[] { '\n' });
                        foreach (var item in lines)
                        {
                            if (item.IsNullOrWhiteSpace())
                                continue;

                            var line = item.Trim().Trim('\r');
                            var arr = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length >= 2)
                            {
                                r.Datas.Add(new BankModel
                                {
                                    Code = arr[0].Trim(),
                                    Name = arr[1].Trim(),
                                    Logo = $"https://apimg.alipay.com/combo.png?d=cashier&t={arr[0]}"
                                });
                            }
                        }
                    }
                });
            }

            return kernelBuilder;
        }
    }
}
