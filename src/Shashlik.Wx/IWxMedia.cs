using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Wx
{
    public interface IWxMedia
    {
        /// <summary>
        /// 获取微信临时素材文件
        /// </summary>
        /// <param name="mediaId">文件serverid</param>
        /// <returns></returns>
        Task<WxMediaFile> GetFile(string mediaId);
    }

    public class WxMediaFile
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件字节数据
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// HttpContentType
        /// </summary>
        public string HttpContentType { get; set; }
    }
}
