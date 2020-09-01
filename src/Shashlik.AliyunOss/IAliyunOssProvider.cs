using System;
using System.Collections.Generic;

namespace Shashlik.AliyunOss
{
    //TODO: 支持多文件上传
    
    public interface IAliyunOssProvider
    {
        /// <summary>
        /// 验证文件类型,仅基于简单的扩展名校验
        /// </summary>
        /// <param name="ext">文件扩展名,不包含.</param>
        /// <returns></returns>
        (bool success, string errorMsg) ValidExt(string ext);

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