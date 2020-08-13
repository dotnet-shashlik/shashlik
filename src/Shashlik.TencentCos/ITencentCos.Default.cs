using COSXML;
using COSXML.Auth;
using COSXML.Model;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Service;
using COSXML.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ms.V20180408;
using TencentCloud.Ms.V20180408.Models;
using TencentCloud.Sts.V20180813;
using TencentCloud.Sts.V20180813.Models;

namespace TencentCos.Sdk
{
    /// <summary>
    /// 腾讯云cos文件操作接口
    /// </summary>
    public class DefaultTencentCos : ITencentCos
    {
        public DefaultTencentCos(
            IOptionsSnapshot<TencentCosOptions> options
            )
        {
            Options = options;
            XmlServer = new Lazy<CosXmlServer>(() =>
            {
                return InitCredential(GetLongCredential());
            });
        }

        IOptionsSnapshot<TencentCosOptions> Options { get; }
        Lazy<CosXmlServer> XmlServer { get; }

        public void Down(string fileKey, string localFilePath)
        {
            string localDir = Path.GetDirectoryName(localFilePath);//下载到本地指定文件夹
            string localFileName = Path.GetFileName(localFilePath); //指定本地保存的文件名
            GetObjectRequest request = new GetObjectRequest(Options.Value.BucketName, fileKey, localDir, localFileName);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
            //执行请求
            GetObjectResult result = XmlServer.Value.GetObject(request);

            if (!IsSuccess(result))
                throw new Exception($"cos文件下载请求不成功:{result.httpCode},{result.httpMessage}");
        }

        public List<TencentCosFileModel> Get()
        {
            GetBucketRequest request = new GetBucketRequest(Options.Value.BucketName);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
            //执行请求
            GetBucketResult result = XmlServer.Value.GetBucket(request);

            if (!IsSuccess(result))
                throw new Exception($"cos文件获取文件列表请求不成功:{result.httpCode},{result.httpMessage}");

            if (result.listBucket == null)
                throw new Exception($"文件为空");

            return result.listBucket.contentsList.Select(r => new TencentCosFileModel
            {
                ETag = r.eTag,
                FileKey = r.key,
                IsDir = r.size <= 0,
                LastModify = r.lastModified,
                Size = r.size
            }).ToList();
        }

        public TencentCosTempKeyModel TempKey(string basePathAfterAllowPath, int randomCount)
        {
            if (randomCount > 99)
                throw new Exception("最大数量99");
            List<string> randoms = new List<string>();

            for (int i = 0; i < randomCount; i++)
                randoms.Add(Guid.NewGuid().ToString("n"));


            var key = BuildTempKey(basePathAfterAllowPath);
            return new TencentCosTempKeyModel
            {
                ExpireTime = (long?)key.ExpiredTime,
                TempSecretId = key.Credentials.TmpSecretId,
                TempSecretKey = key.Credentials.TmpSecretKey,
                TempSecretToken = key.Credentials.Token,
                AllowPath = Options.Value.TempKeyGetterPolicy.AllowPath.Trim('/') + "/" + basePathAfterAllowPath.Trim('/'),
                BucketName = Options.Value.BucketName,
                Region = Options.Value.Region,
                Randoms = randoms
            };
        }

        public void Upload(string filePath, string fileKey)
        {
            PutObjectRequest request = new PutObjectRequest(Options.Value.BucketName, fileKey, filePath);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
            //执行请求
            PutObjectResult result = XmlServer.Value.PutObject(request);
            if (!IsSuccess(result))
                throw new Exception($"cos文件上传请求不成功:{result.httpCode},{result.httpMessage}");
        }

        public void Upload(byte[] fileData, string fileKey)
        {
            PutObjectRequest request = new PutObjectRequest(Options.Value.BucketName, fileKey, fileData);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
            //执行请求
            PutObjectResult result = XmlServer.Value.PutObject(request);

            if (!IsSuccess(result))
                throw new Exception($"cos文件上传请求不成功:{result.httpCode},{result.httpMessage}");
        }

        #region private

        /// <summary>
        /// 获取临时密钥
        /// </summary>
        /// <returns></returns>
        GetFederationTokenResponse BuildTempKey(string basePathAfterAllowPath)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = Options.Value.SecretId,
                    SecretKey = Options.Value.SecretKey
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("sts.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                StsClient client = new StsClient(cred, Options.Value.Region, clientProfile);

                GetFederationTokenRequest req = new GetFederationTokenRequest()
                {
                    Name = Options.Value.TempKeyGetterAccountName,
                    Policy = Options.Value.BuildPolicy(basePathAfterAllowPath),
                    DurationSeconds = (ulong?)Options.Value.TempKeyGetterDurationSeconds
                };

                return client.GetFederationToken(req).
                   ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception("获取腾讯云cos临时密钥错误", ex);
            }
        }

        /// <summary>
        /// 获取永久密钥
        /// </summary>
        /// <returns></returns>
        QCloudCredentialProvider GetLongCredential()
        {
            //方式1， 永久密钥
            string secretId = Options.Value.SecretId; //"云 API 密钥 SecretId";
            string secretKey = Options.Value.SecretKey; //"云 API 密钥 SecretKey";
            long durationSecond = Options.Value.TempKeyGetterDurationSeconds ?? 600;  //secretKey 有效时长,单位为 秒
            return new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
        }

        CosXmlServer InitCredential(QCloudCredentialProvider provider)
        {
            //初始化 CosXmlConfig 
            string appid = Options.Value.SecretId;//设置腾讯云账户的账户标识 APPID
            string region = Options.Value.Region; //设置一个默认的存储桶地域
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位 毫秒 ，默认 45000ms
                .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位 毫秒 ，默认 45000ms
                .IsHttps(true)  //设置默认 https 请求
                .SetAppid(appid)  //设置腾讯云账户的账户标识 APPID
                .SetRegion(region)  //设置一个默认的存储桶地域
                .SetDebugLog(true)  //显示日志
                .Build();  //创建 CosXmlConfig 对象


            //初始化 CosXmlServer
            CosXmlServer cosXml = new CosXmlServer(config, provider);
            return cosXml;
        }

        #endregion

        bool IsSuccess(CosResult cosResult)
        {
            return cosResult.httpCode >= 200 && cosResult.httpCode <= 299;
        }
    }
}
