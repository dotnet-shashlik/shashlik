// copy and modified from: https://github.com/xiangyuecn/RSA-csharp

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Shashlik.Utils.Helpers.RsaInner
{
    public class RSAParametersConvert
    {
        internal static readonly Regex PemCode = new Regex(@"--+.+?--+|\s+");

        internal static readonly byte[] SeqOid = new byte[]
            {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};

        internal static readonly byte[] Ver = new byte[] {0x02, 0x01, 0x00};
        internal static readonly Regex XmlExp = new Regex("\\s*<RSAKeyValue>([<>\\/\\+=\\w\\s]+)</RSAKeyValue>\\s*");
        internal static readonly Regex XmlTagExp = new Regex("<(.+?)>\\s*([^<]+?)\\s*</");

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


        /// <summary>
        /// 导出为xml key
        /// </summary>
        /// <param name="rsaParameters"></param>
        /// <param name="includePrivateKey">是否导出私钥</param>
        /// <returns></returns>
        public static string ToXml(RSAParameters rsaParameters, bool includePrivateKey)
        {
            StringBuilder str = new StringBuilder();
            str.Append("<RSAKeyValue>");
            str.Append("<Modulus>" + Convert.ToBase64String(rsaParameters.Modulus) + "</Modulus>");
            str.Append("<Exponent>" + Convert.ToBase64String(rsaParameters.Exponent) + "</Exponent>");
            if (rsaParameters.D != null && includePrivateKey)
            {
                str.Append("<P>" + Convert.ToBase64String(rsaParameters.P) + "</P>");
                str.Append("<Q>" + Convert.ToBase64String(rsaParameters.Q) + "</Q>");
                str.Append("<DP>" + Convert.ToBase64String(rsaParameters.DP) + "</DP>");
                str.Append("<DQ>" + Convert.ToBase64String(rsaParameters.DQ) + "</DQ>");
                str.Append("<InverseQ>" + Convert.ToBase64String(rsaParameters.InverseQ) + "</InverseQ>");
                str.Append("<D>" + Convert.ToBase64String(rsaParameters.D) + "</D>");
            }

            str.Append("</RSAKeyValue>");
            return str.ToString();
        }

        /// <summary>
        /// 导出为pem key
        /// </summary>
        /// <param name="rsaParameters"></param>
        /// <param name="includePrivateKey">是否导出私钥</param>
        /// <param name="isPkcs8">是否导出为pkcs8格式</param>
        /// <returns></returns>
        public static string ToPem(RSAParameters rsaParameters, bool includePrivateKey, bool isPkcs8)
        {
            var ms = new MemoryStream();
            //写入一个长度字节码
            Action<int> writeLenByte = (len) =>
            {
                if (len < 0x80)
                {
                    ms.WriteByte((byte) len);
                }
                else if (len <= 0xff)
                {
                    ms.WriteByte(0x81);
                    ms.WriteByte((byte) len);
                }
                else
                {
                    ms.WriteByte(0x82);
                    ms.WriteByte((byte) (len >> 8 & 0xff));
                    ms.WriteByte((byte) (len & 0xff));
                }
            };
            //写入一块数据
            Action<byte[]> writeBlock = (byts) =>
            {
                var addZero = (byts[0] >> 4) >= 0x8;
                ms.WriteByte(0x02);
                var len = byts.Length + (addZero ? 1 : 0);
                writeLenByte(len);

                if (addZero)
                {
                    ms.WriteByte(0x00);
                }

                ms.Write(byts, 0, byts.Length);
            };
            //根据后续内容长度写入长度数据
            Func<int, byte[], byte[]> writeLen = (index, byts) =>
            {
                var len = byts.Length - index;

                ms.SetLength(0);
                ms.Write(byts, 0, index);
                writeLenByte(len);
                ms.Write(byts, index, len);

                return ms.ToArray();
            };
            Action<MemoryStream, byte[]> writeAll = (stream, byts) => { stream.Write(byts, 0, byts.Length); };
            Func<string, int, string> textBreak = (text, line) =>
            {
                var idx = 0;
                var len = text.Length;
                var str = new StringBuilder();
                while (idx < len)
                {
                    if (idx > 0)
                    {
                        str.Append('\n');
                    }

                    if (idx + line >= len)
                    {
                        str.Append(text.Substring(idx));
                    }
                    else
                    {
                        str.Append(text.Substring(idx, line));
                    }

                    idx += line;
                }

                return str.ToString();
            };

            if (rsaParameters.D == null || !includePrivateKey)
            {
                /****生成公钥****/

                //写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
                ms.WriteByte(0x30);
                var index1 = (int) ms.Length;

                //PKCS8 多一段数据
                int index2 = -1, index3 = -1;
                if (isPkcs8)
                {
                    //固定内容
                    // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                    writeAll(ms, RSAParametersConvert.SeqOid);

                    //从0x00开始的后续长度
                    ms.WriteByte(0x03);
                    index2 = (int) ms.Length;
                    ms.WriteByte(0x00);

                    //后续内容长度
                    ms.WriteByte(0x30);
                    index3 = (int) ms.Length;
                }

                //写入Modulus
                writeBlock(rsaParameters.Modulus);

                //写入Exponent
                writeBlock(rsaParameters.Exponent);


                //计算空缺的长度
                var byts = ms.ToArray();

                if (index2 != -1)
                {
                    byts = writeLen(index3, byts);
                    byts = writeLen(index2, byts);
                }

                byts = writeLen(index1, byts);


                var flag = " PUBLIC KEY";
                if (!isPkcs8)
                {
                    flag = " RSA" + flag;
                }

                return "-----BEGIN" + flag + "-----\n" + textBreak(Convert.ToBase64String(byts), 64) + "\n-----END" + flag + "-----";
            }
            else
            {
                /****生成私钥****/

                //写入总字节数，后续写入
                ms.WriteByte(0x30);
                int index1 = (int) ms.Length;

                //写入版本号
                writeAll(ms, RSAParametersConvert.Ver);

                //PKCS8 多一段数据
                int index2 = -1, index3 = -1;
                if (isPkcs8)
                {
                    //固定内容
                    writeAll(ms, RSAParametersConvert.SeqOid);

                    //后续内容长度
                    ms.WriteByte(0x04);
                    index2 = (int) ms.Length;

                    //后续内容长度
                    ms.WriteByte(0x30);
                    index3 = (int) ms.Length;

                    //写入版本号
                    writeAll(ms, RSAParametersConvert.Ver);
                }

                //写入数据
                writeBlock(rsaParameters.Modulus);
                writeBlock(rsaParameters.Exponent);
                writeBlock(rsaParameters.D);
                writeBlock(rsaParameters.P);
                writeBlock(rsaParameters.Q);
                writeBlock(rsaParameters.DP);
                writeBlock(rsaParameters.DQ);
                writeBlock(rsaParameters.InverseQ);


                //计算空缺的长度
                var byts = ms.ToArray();

                if (index2 != -1)
                {
                    byts = writeLen(index3, byts);
                    byts = writeLen(index2, byts);
                }

                byts = writeLen(index1, byts);


                var flag = " PRIVATE KEY";
                if (!isPkcs8)
                {
                    flag = " RSA" + flag;
                }

                return "-----BEGIN" + flag + "-----\n" + textBreak(Convert.ToBase64String(byts), 64) + "\n-----END" + flag + "-----";
            }
        }
    }
}