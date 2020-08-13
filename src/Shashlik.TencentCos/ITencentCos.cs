using System;
using System.Collections.Generic;
using System.Text;

namespace TencentCos.Sdk
{
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    public interface ITencentCos
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="fileKey">云存储绝对路径</param>
        void Upload(string filePath, string fileKey);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="fileKey">云存储绝对路径</param>
        void Upload(byte[] fileData, string fileKey);

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileKey">云存储路径</param>
        /// <param name="localFilePath">下载后存储路径</param>
        void Down(string fileKey, string localFilePath);

        /// <summary>
        /// 获取所有的文件列表
        /// </summary>
        /// <returns></returns>
        List<TencentCosFileModel> Get();

        /// <summary>
        /// 获取临时密钥
        /// </summary>
        /// <returns></returns>
        TencentCosTempKeyModel TempKey(string basePathAfterAllowPath, int randomCount);
    }

    public class TencentCosFileModel
    {
        /// <summary>
        /// 文件key,存储路径
        /// </summary>
        public string FileKey { get; set; }

        /// <summary>
        /// etag
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDir { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastModify { get; set; }
    }

    public class TencentCosTempKeyModel
    {
        /// <summary>
        /// 临时密钥appid
        /// </summary>
        public string TempSecretId { get; set; }
        /// <summary>
        /// 临时密钥key
        /// </summary>
        public string TempSecretKey { get; set; }
        /// <summary>
        /// 签名token
        /// </summary>
        public string TempSecretToken { get; set; }
        /// <summary>
        /// 密钥过期时间
        /// </summary>
        public long? ExpireTime { get; set; }
        /// <summary>
        /// 临时密钥 允许上传的目录,如前端需要上传的文件名为1.jpg,最后调用sdk时上传的参数为{AllowPath}+'/'+1.jpg
        /// </summary>
        public string AllowPath { get; set; }

        /// <summary>
        /// 存储桶的名称
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 存储桶地区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 随机字符传,用于文件命名
        /// </summary>
        public List<string> Randoms { get; set; }
    }
}
