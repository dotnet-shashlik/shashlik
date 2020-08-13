using Guc.Utils.Extensions;
using Guc.Utils.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guc.Bank
{
    class DefaultBankService : IBankService, Guc.Kernel.Dependency.ISingleton
    {
        public DefaultBankService(IOptions<BankOptions> options)
        {
            Options = options;
        }

        IOptions<BankOptions> Options { get; }

        public List<BankModel> All()
        {
            return Options.Value.Datas;
        }

        public BankModel Get(string code)
        {
            return Options.Value.Datas.FirstOrDefault(r => r.Code == code);
        }

        public bool TryGetByCardNo(string bankCardNo, out BankCardModel cardModel)
        {
            cardModel = new BankCardModel
            {
                CardType = CardType.Unknown
            };

            if (string.IsNullOrWhiteSpace(bankCardNo))
                return false;

            try
            {
                var resultStr =
                HttpHelper.GetString($"https://ccdcapi.alipay.com/validateAndCacheCardInfo.json?_input_charset=utf-8&cardNo={bankCardNo}&cardBinCheck=true")
                    .GetAwaiter().GetResult();

                var obj = JsonHelper.Deserialize<JObject>(resultStr);

                if (!obj["validated"].Value<bool>())
                    return false;

                cardModel.CardType = (CardType)Enum.Parse(typeof(CardType), obj["cardType"].Value<string>());
                var code = obj["bank"].Value<string>();

                var bank = Options.Value.Datas.FirstOrDefault(r => r.Code == code);
                cardModel.Code = bank.Code;
                cardModel.Logo = bank.Logo;
                cardModel.Name = bank.Name;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
