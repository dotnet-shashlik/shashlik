using System;

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 国内身份证相关
    /// </summary>
    public static class IdCardHelper
    {
        /// <summary>
        /// 验证身份证,不支持15位身份证
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        public static bool IsIdCard(this string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
                return false;
            if (idCard.Length != 18)
                return false;
            bool result;
            try
            {
                if (!long.TryParse(idCard.Remove(17), out var num) || num < Math.Pow(10.0, 16.0) || !long.TryParse(idCard.Replace('x', '0').Replace('X', '0'), out num))
                {
                    result = false;
                }
                else if ("11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91".IndexOf(idCard.Remove(2)) == -1)
                {
                    result = false;
                }
                else
                {
                    string s = idCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                    if (!DateTime.TryParse(s, out _))
                    {
                        result = false;
                    }
                    else
                    {
                        string[] array = "1,0,x,9,8,7,6,5,4,3,2".Split(new char[]
                        {
                            ','
                        });
                        string[] array2 = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(new char[]
                        {
                            ','
                        });
                        char[] array3 = idCard.Remove(17).ToCharArray();
                        int num2 = 0;
                        for (int i = 0; i < 17; i++)
                        {
                            num2 += int.Parse(array2[i]) * int.Parse(array3[i].ToString());
                        }
                        int num3 = -1;
                        Math.DivRem(num2, 11, out num3);
                        if (array[num3] != idCard.Substring(17, 1).ToLower())
                        {
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取身份证中的基础数据:性别,年龄,生日
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static IdCardModel GetIdCardModel(string idCard)
        {
            if (!IsIdCard(idCard))
                return null;

            IdCardModel result;
            try
            {
                var cardIdModel = new IdCardModel
                {
                    IdCard = idCard,
                    Birthday = Convert.ToDateTime(idCard.Substring(6, 8).Insert(6, "-").Insert(4, "-"))
                };
                Math.DivRem(int.Parse(idCard.Substring(idCard.Length - 4, 3)), 2, out var num);
                cardIdModel.Sex = sbyte.Parse(num.ToString());
                DateTime now = DateTime.Now;
                int num2 = now.Year - cardIdModel.Birthday.Year;
                if (now.Month < cardIdModel.Birthday.Month || (now.Month == cardIdModel.Birthday.Month && now.Day < cardIdModel.Birthday.Day))
                {
                    num2--;
                }
                cardIdModel.Age = ((num2 < 0) ? 0 : num2);
                result = cardIdModel;
            }
            catch
            {
                result = null;
            }
            return result;
        }

    }
    /// <summary>
    /// 身份证模型
    /// </summary>
    public class IdCardModel
    {
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 性别0:女,1:男,2未知
        /// </summary>
        public sbyte Sex { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }
    }
}
