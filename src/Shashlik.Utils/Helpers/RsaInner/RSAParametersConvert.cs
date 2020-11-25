// copy and modified from: https://github.com/xiangyuecn/RSA-csharp

using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Shashlik.Utils.Helpers.RsaInner
{
    public class RSAParametersConvert
    {
        private static readonly Regex PemCode = new Regex(@"--+.+?--+|\s+");

        private static readonly byte[] SeqOid = new byte[]
            {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};

        private static readonly byte[] Ver = new byte[] {0x02, 0x01, 0x00};
        private static readonly Regex XmlExp = new Regex("\\s*<RSAKeyValue>([<>\\/\\+=\\w\\s]+)</RSAKeyValue>\\s*");
        private static readonly Regex XmlTagExp = new Regex("<(.+?)>\\s*([^<]+?)\\s*</");

        /// <summary>
        /// 某些密钥参数可能会少一位（32个byte只有31个，目测是密钥生成器的问题，只在c#生成的密钥中发现这种参数，java中生成的密钥没有这种现象），直接修正一下就行；这个问题与BigB有本质区别，不能动BigB
        /// </summary>
        public static byte[] BigL(byte[] bytes, int keyLen)
        {
            if (keyLen - bytes.Length == 1)
            {
                byte[] c = new byte[bytes.Length + 1];
                Array.Copy(bytes, 0, c, 1, bytes.Length);
                bytes = c;
            }

            return bytes;
        }

        public static RSAParameters PemToRSAParameters(string pem)
        {
            var rsaParameters = new RSAParameters();
            var base64 = PemCode.Replace(pem, "");
            byte[] data = null;
            try
            {
                data = Convert.FromBase64String(base64);
            }
            catch
            {
                // ignored
            }

            if (data is null)
                throw new ArgumentException("invalid pem key", nameof(pem));

            var idx = 0;

            //读取长度
            Func<byte, int> readLen = (first) =>
            {
                // ReSharper disable once AccessToModifiedClosure
                if (data[idx] == first)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    idx++;
                    if (data[idx] == 0x81)
                    {
                        idx++;
                        return data[idx++];
                    }
                    else if (data[idx] == 0x82)
                    {
                        idx++;
                        return (((int) data[idx++]) << 8) + data[idx++];
                    }
                    else if (data[idx] < 0x80)
                    {
                        return data[idx++];
                    }
                }

                throw new ArgumentException("invalid pem key", nameof(pem));
            };
            //读取块数据
            Func<byte[]> readBlock = () =>
            {
                var len = readLen(0x02);
                if (data[idx] == 0x00)
                {
                    idx++;
                    len--;
                }

                var val = new byte[len];
                for (var i = 0; i < len; i++)
                {
                    val[i] = data[idx + i];
                }

                idx += len;
                return val;
            };
            //比较data从idx位置开始是否是byts内容
            Func<byte[], bool> eq = (byts) =>
            {
                for (var i = 0; i < byts.Length; i++, idx++)
                {
                    if (idx >= data.Length)
                    {
                        return false;
                    }

                    if (byts[i] != data[idx])
                    {
                        return false;
                    }
                }

                return true;
            };

            if (pem.Contains("PUBLIC KEY"))
            {
                /****使用公钥****/
                //读取数据总长度
                readLen(0x30);

                //检测PKCS8
                var idx2 = idx;
                if (eq(SeqOid))
                {
                    //读取1长度
                    readLen(0x03);
                    idx++; //跳过0x00
                    //读取2长度
                    readLen(0x30);
                }
                else
                {
                    idx = idx2;
                }

                //Modulus
                rsaParameters.Modulus = readBlock();
                //Exponent
                rsaParameters.Exponent = readBlock();
            }
            else if (pem.Contains("PRIVATE KEY"))
            {
                /****使用私钥****/
                //读取数据总长度
                readLen(0x30);

                //读取版本号
                if (!eq(Ver))
                {
                    throw new ArgumentException("invalid pem key", nameof(pem));
                }

                //检测PKCS8
                var idx2 = idx;
                if (eq(SeqOid))
                {
                    //读取1长度
                    readLen(0x04);
                    //读取2长度
                    readLen(0x30);

                    //读取版本号
                    if (!eq(Ver))
                    {
                        throw new ArgumentException("invalid pem key", nameof(pem));
                    }
                }
                else
                {
                    idx = idx2;
                }

                //读取数据
                rsaParameters.Modulus = readBlock();
                rsaParameters.Exponent = readBlock();
                int keyLen = rsaParameters.Modulus.Length;
                rsaParameters.D = BigL(readBlock(), keyLen);
                keyLen = keyLen / 2;
                rsaParameters.P = BigL(readBlock(), keyLen);
                rsaParameters.Q = BigL(readBlock(), keyLen);
                rsaParameters.DP = BigL(readBlock(), keyLen);
                rsaParameters.DQ = BigL(readBlock(), keyLen);
                rsaParameters.InverseQ = BigL(readBlock(), keyLen);
            }
            else
                throw new ArgumentException("invalid pem key", nameof(pem));

            return rsaParameters;
        }

        /// <summary>
        /// 将XML格式密钥转成<see cref="RSAParameters"/>>，支持公钥xml、私钥xml
        /// 出错将会抛出异常
        /// </summary>
        public static RSAParameters XmlToRSAParameters(string xml)
        {
            var rsaParameters = new RSAParameters();

            Match xmlM = XmlExp.Match(xml);
            if (!xmlM.Success)
                throw new ArgumentException("invalid xml key", nameof(xml));

            Match tagM = XmlTagExp.Match(xmlM.Groups[1].Value);
            while (tagM.Success)
            {
                string tag = tagM.Groups[1].Value;
                string b64 = tagM.Groups[2].Value;
                byte[] val = Convert.FromBase64String(b64);
                switch (tag)
                {
                    case "Modulus":
                        rsaParameters.Modulus = val;
                        break;
                    case "Exponent":
                        rsaParameters.Exponent = val;
                        break;
                    case "D":
                        rsaParameters.D = val;
                        break;
                    case "P":
                        rsaParameters.P = val;
                        break;
                    case "Q":
                        rsaParameters.Q = val;
                        break;
                    case "DP":
                        rsaParameters.DP = val;
                        break;
                    case "DQ":
                        rsaParameters.DQ = val;
                        break;
                    case "InverseQ":
                        rsaParameters.InverseQ = val;
                        break;
                }

                tagM = tagM.NextMatch();
            }

            if (rsaParameters.Modulus == null || rsaParameters.Exponent == null)
            {
                throw new ArgumentException("invalid xml key", nameof(xml));
            }

            if (rsaParameters.D != null)
            {
                if (rsaParameters.P == null || rsaParameters.Q == null || rsaParameters.DP == null || rsaParameters.DQ == null ||
                    rsaParameters.InverseQ == null)
                {
                    rsaParameters.P = null;
                    rsaParameters.Q = null;
                    rsaParameters.DP = null;
                    rsaParameters.DQ = null;
                    rsaParameters.InverseQ = null;
                }
            }

            return rsaParameters;
        }
    }
}