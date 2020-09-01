using Aliyun.OSS;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Utils.Extensions;

// ReSharper disable IdentifierTypo

namespace Shashlik.AliyunOss
{
    [ConditionOnProperty("Shashlik:AliyunOss:Enable", "true")]
    public class AliyunOssDefaultProvider : IAliyunOssProvider, ISingleton
    {
        private OssClient Client { get; }
        private AliyunOssOptions Options { get; }

        public AliyunOssDefaultProvider(IOptions<AliyunOssOptions> options)
        {
            Options = options.Value;
            Client = new OssClient(options.Value.Endpoint, options.Value.AccessId, options.Value.AccessKey);
        }

        public (bool success, string errorMsg) ValidExt(string ext)
        {
            if (ext.IsNullOrWhiteSpace())
                return (false, "文件扩展名为空");
            ext = ext.Trim();
            if (!Options.FileExtLimit.IsNullOrEmpty() && !Options.FileExtLimit.Any(r => r.EqualsIgnoreCase(ext)))
                return (false, "错误的文件类型");
            return (true, null);
        }

        public AliyunOssPostPolicy BuildPolicy(string ext)
        {
            var maxSize = 8;
            if (Options.MaxSize > 0)
                maxSize = Options.MaxSize;

            if (ext.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(ext));
            ext = ext.Trim();
            if (!Options.FileExtLimit.IsNullOrEmpty() && !Options.FileExtLimit.Any(r => r.EqualsIgnoreCase(ext)))
                throw new ArgumentException("错误的文件类型", nameof(ext));
            if (!ext.StartsWith("."))
                ext = $".{ext}";
            var dir = $"{Options.Dir}{DateTime.Now:yyyyMMdd}/";
            var fileName = $"{Guid.NewGuid():n}{ext}";
            var expiration = DateTime.Now.AddMinutes(1);
            var policyConditions = new PolicyConditions();
            policyConditions.AddConditionItem("bucket", Options.Bucket);
            // $ must be escaped with backslash.
            var key = $"{dir}{fileName}";
            policyConditions.AddConditionItem(MatchMode.Exact, PolicyConditions.CondKey, key);
            policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, maxSize * 1024 * 1024); //大小限制

            var postPolicy = Client.GeneratePostPolicy(expiration, policyConditions);
            var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));

            var signature = ComputeSignature(Options.AccessKey, encPolicy);

            return new AliyunOssPostPolicy
            {
                AccessId = Options.AccessId,
                Dir = dir,
                Expire = expiration.GetIntDate(),
                Policy = encPolicy,
                Signature = signature,
                Host = Options.Host,
                FileName = fileName,
                Key = key,
                Url = $"{Options.CdnHost.TrimEnd('/')}/{key}"
            };
        }

        private string ComputeSignature(string key, string data)
        {
            using var algorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant());
            algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
            return Convert.ToBase64String(
                algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
        }

        public string Upload(string objectName, string localFullFileName)
        {
            Client.PutObject(Options.Bucket, objectName, localFullFileName);

            return $"{Options.CdnHost.TrimEnd('/')}/{objectName}";
        }

        public string Upload(string objectName, byte[] fileContent)
        {
            using var requestContent = new MemoryStream(fileContent);
            // 上传文件。
            Client.PutObject(Options.Bucket, objectName, requestContent);
            return $"{Options.CdnHost.TrimEnd('/')}/{objectName}";
        }
    }
}