using Aliyun.OSS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;
using System.IO;
using Shashlik.Kernel.Dependency;

namespace Shashlik.AliyunOss
{
    public class AliyunOssDefaultProvider : IAliyunOssProvider, ISingleton
    {
        private OssClient client { get; }
        private AliyunOssOptions options { get; }

        public AliyunOssDefaultProvider(IOptions<AliyunOssOptions> options)
        {
            this.options = options.Value;
            client = new OssClient(options.Value.Endpoint, options.Value.AccessId, options.Value.AccessKey);
        }

        public AliyunOssPostPolicy BuildPolicy(string ext)
        {
            int maxSize = 8;
            if (options.MaxSize > 0)
                maxSize = options.MaxSize;

            if (ext.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(ext));
            ext = ext.Trim();
            if (!ext.StartsWith("."))
                ext = $".{ext}";
            var dir = $"{options.Dir}{DateTime.Now:yyyyMMdd}/";
            var fileName = $"{Guid.NewGuid():n}{ext}";
            var expiration = DateTime.Now.AddMinutes(1);
            var policyConditions = new PolicyConditions();
            policyConditions.AddConditionItem("bucket", options.Bucket);
            // $ must be escaped with backslash.
            var key = $"{dir}{fileName}";
            //TODO: 增加contentType上传类型限制
            policyConditions.AddConditionItem(MatchMode.Exact, PolicyConditions.CondKey, key);
            policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, maxSize * 1024 * 1024);//大小限制

            var postPolicy = client.GeneratePostPolicy(expiration, policyConditions);
            var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));

            var signature = ComputeSignature(options.AccessKey, encPolicy);

            return new AliyunOssPostPolicy
            {
                AccessId = options.AccessId,
                Dir = dir,
                Expire = expiration.GetIntDate(),
                Policy = encPolicy,
                Signature = signature,
                Host = options.Host,
                FileName = fileName,
                Key = key,
                Url = $"{options.CdnHost.TrimEnd('/')}/{key}"
            };
        }

        private string ComputeSignature(string key, string data)
        {
            using (var algorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant()))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
            }
        }

        public string Upload(string objectName, string localFullFileName)
        {
            client.PutObject(options.Bucket, objectName, localFullFileName);

            return $"{options.CdnHost.TrimEnd('/')}/{objectName}";
        }

        public string Upload(string objectName, byte[] fileContent)
        {
            using (var requestContent = new MemoryStream(fileContent))
            {
                // 上传文件。
                client.PutObject(options.Bucket, objectName, requestContent);
                return $"{options.CdnHost.TrimEnd('/')}/{objectName}";
            }
        }
    }
}
