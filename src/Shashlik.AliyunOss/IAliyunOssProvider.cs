using System;
using System.Collections.Generic;

namespace Guc.AliyunOss
{
    public interface IAliyunOssProvider
    {
        /// <summary>
        /// 生成上传策略
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        AliyunOssPostPolicy BuildPolicy(string ext);

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
    }
}
