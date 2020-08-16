using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Bank
{
    public interface IBankService
    {
        /// <summary>
        /// 所有的银行数据
        /// </summary>
        /// <returns></returns>
        List<BankModel> All();

        /// <summary>
        /// 根据code获取银行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        BankModel Get(string code);

        /// <summary>
        /// 根据银行卡号自动计算银行卡信息
        /// </summary>
        /// <param name="bankCardNo"></param>
        /// <param name="cardModel"></param>
        /// <returns></returns>
        bool TryGetByCardNo(string bankCardNo, out BankCardModel cardModel);
    }
}
