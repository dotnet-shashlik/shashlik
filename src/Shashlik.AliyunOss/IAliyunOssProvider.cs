using System.Collections.Generic;
using System.IO;

namespace Shashlik.AliyunOss
{
    public interface IAliyunOssProvider
    {
        /// <summary>
        /// 以存储桶的名称区分,不同的实例
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// 生成单文件上传策略,会限定上传的文件名
        /// </summary>
        /// <param name="fileKey">完整的文件key,OSS文件路径</param>
        /// <param name="expireSeconds">策略过期时间</param>
        /// <param name="maxSize">文件大小限制</param>
        /// <returns></returns>
        AliyunOssPostPolicy BuildSingleFilePolicy(string fileKey, int expireSeconds = 60,
            int maxSize = 8 * 1024 * 1024);

        /// <summary>
        /// 生成单文件上传策略
        /// </summary>
        /// <param name="fileKeyStartWith">完整的文件key前缀</param>
        /// <param name="expireSeconds">策略过期时间</param>
        /// <param name="maxSize">文件大小限制</param>
        /// <returns></returns>
        AliyunOssPostPolicy BuildStartWithPolicy(string fileKeyStartWith, int expireSeconds = 60,
            int maxSize = 8 * 1024 * 1024);

        /// <summary>
        /// 文件本地文件
        /// </summary>
        /// <param name="objectName">路径</param>
        /// <param name="localFullFileName">本地文件路径</param>
        /// <returns></returns>
        string Upload(string objectName, string localFullFileName);

        /// <summary>
        /// 上传文件字节数组
        /// </summary>
        /// <param name="objectName">路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <returns></returns>
        string Upload(string objectName, byte[] fileContent);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        Stream Download(string objectName);

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        bool Exists(string objectName);

        /// <summary>
        /// url文件签名访问
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="expires">过期时间,单位秒</param>
        /// <returns></returns>
        string SignatureUrl(string objectName, int expires = 3600);
    }
}