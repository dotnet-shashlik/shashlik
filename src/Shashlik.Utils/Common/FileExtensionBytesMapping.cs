using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Guc.Utils.Extensions;

namespace Guc.Utils.Common
{
    /// <summary>
    /// 文件类型-字节数组映射
    /// </summary>
    public class FileExtensionBytesMapping
    {
        private static readonly Dictionary<string, byte[]> mappings = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
        {
           {"jpg"     , new byte[] { 0xFF, 0xD8 } } ,
           {"jpeg"    , new byte[] { 0xFF, 0xD8 } },
           {"bmp"     , new byte[] { 0x42, 0x4D } },
           {"png"     , new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } ,
           {"gif"     , new byte[] { 0x47, 0x49, 0x46, 0x38 } } ,
           {"tif"     , new byte[] { 0x49, 0x20, 0x49 } } ,
           {"tiff"    , new byte[] { 0x49, 0x20, 0x49 } },
           {"docx"    , new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } ,
           {"pptx"    , new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } ,
           {"xlsx"    , new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } ,
           {"doc"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"dot"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"pps"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"ppt"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"xla"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"xls"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"msi"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"wps"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"vsd"     , new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } ,
           {"pdf"     , new byte[] { 0x25, 0x50, 0x44, 0x46 } } ,
           {"zip"     , new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
           {"rar"     , new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07 } },
           {"7z"      , new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C } },
           {"psd"     , new byte[] { 0x38, 0x42, 0x50, 0x53 } },
           {"mp3"     , new byte[] { 0x49, 0x44, 0x33 } },
           {"amr"     , new byte[] { 0x23, 0x21, 0x41, 0x4D, 0x52 } },
           {"wav"     , new byte[] { 0x52, 0x49, 0x46, 0x46 } },
           {"rmi"     , new byte[] { 0x52, 0x49, 0x46, 0x46 } },
           {"qcp"     , new byte[] { 0x52, 0x49, 0x46, 0x46 } },
           {"avi"     , new byte[] { 0x52, 0x49, 0x46, 0x46 } },
           {"cda"     , new byte[] { 0x52, 0x49, 0x46, 0x46 } }

        };

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="extension">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatch(Stream fileStream, params string[] extensions)
        {
            if (!fileStream.CanRead)
                throw new Exception("stream CanRead is false");

            var data = new byte[20];
            fileStream.Read(data, 0, data.Length);
            if (fileStream.CanSeek)
                fileStream.Seek(0, SeekOrigin.Begin);

            foreach (var extension in extensions)
            {
                if (!mappings.ContainsKey(extension))
                    throw new Exception($"not support file extension:{extension} .");

                var valBytes = mappings[extension];

                var isMatch = true;
                for (int i = 0; i < valBytes.Length; i++)
                {
                    if (data[i] != valBytes[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="extension">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatch(Stream fileStream, out string matchExtension, params string[] extensions)
        {
            if (!fileStream.CanRead)
                throw new Exception("stream CanRead is false");
            matchExtension = "";

            var data = new byte[20];
            fileStream.Read(data, 0, data.Length);
            if (fileStream.CanSeek)
                fileStream.Seek(0, SeekOrigin.Begin);

            foreach (var extension in extensions)
            {
                if (!mappings.ContainsKey(extension))
                    throw new Exception($"not support file extension:{extension} .");

                var valBytes = mappings[extension];

                var isMatch = true;
                for (int i = 0; i < valBytes.Length; i++)
                {
                    if (data[i] != valBytes[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                {
                    matchExtension = extension;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="extension">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatch(byte[] data, params string[] extensions)
        {
            foreach (var extension in extensions)
            {
                if (!mappings.ContainsKey(extension))
                    throw new Exception($"not support file extension:{extension} .");

                var valBytes = mappings[extension];

                var isMatch = true;
                for (int i = 0; i < valBytes.Length; i++)
                {
                    if (data[i] != valBytes[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="extension">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatch(byte[] data, out string matchExtension, params string[] extensions)
        {
            matchExtension = "";

            foreach (var extension in extensions)
            {
                if (!mappings.ContainsKey(extension))
                    throw new Exception($"not support file extension:{extension} .");

                var valBytes = mappings[extension];

                var isMatch = true;
                for (int i = 0; i < valBytes.Length; i++)
                {
                    if (data[i] != valBytes[i])
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                {
                    matchExtension = extension;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="url">文件url</param>
        /// <param name="extensions">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatch(string url, params string[] extensions)
        {
            if (url.IsNullOrWhiteSpace())
                return false;
            var reg = "(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
            if (!url.IsMatch(reg))
                return false;

            using (var stream = HttpHelper.GetStream(url).GetAwaiter().GetResult())
            {
                return IsMatch(stream, extensions);
            }
        }

        /// <summary>
        /// 文件内容是否符合扩展名
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="extension">扩展名,不区分大小写,不带小数点</param>
        /// <returns></returns>
        public static bool IsMatchFromFile(string filePath, params string[] extensions)
        {
            using (var fs = File.Open(filePath, FileMode.Open))
            {
                return IsMatch(fs, extensions);
            }
        }

        /// <summary>
        /// 根据文件流获取文件扩展名称,找不到返回null
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetExtensions(byte[] data)
        {
            foreach (var item in mappings)
            {
                if (data.Length < item.Value.Length)
                    continue;

                bool isMatch = true;
                for (int i = 0; i < item.Value.Length; i++)
                {
                    var b = item.Value[i];
                    if (b != data[i])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                    return item.Key;
            }

            return null;
        }
    }
}
