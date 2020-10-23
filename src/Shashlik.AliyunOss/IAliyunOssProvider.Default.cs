using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Aliyun.OSS;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

// ReSharper disable IdentifierTypo

namespace Shashlik.AliyunOss
{
    public class AliyunOssDefaultProvider : IAliyunOssProvider
    {
        public string BucketName { get; }
        private OssClient Client { get; }
        private AliyunOssOptions._Buckets Options { get; }

        public AliyunOssDefaultProvider(AliyunOssOptions._Buckets options)
        {
            BucketName = options.Bucket;
            this.Options = options;
            Client = new OssClient(options.Endpoint, options.AccessId, options.AccessKey);
        }

        public AliyunOssPostPolicy BuildSingleFilePolicy(string fileKey, int expireSeconds = 60,
            int maxSize = 8 * 1024 * 1024, IEnumerable<string> policyFileExtLimit = null)
        {
            fileKey = fileKey.TrimStart('/');

            var ext = Path.GetExtension(fileKey).TrimStart('.');
            if (!policyFileExtLimit.IsNullOrEmpty() &&
                !policyFileExtLimit.Any(r => r.EqualsIgnoreCase(ext)))
                throw new ArgumentException("错误的文件类型", nameof(ext));

            var expiration = DateTime.Now.AddSeconds(expireSeconds);
            var policyConditions = new PolicyConditions();
            policyConditions.AddConditionItem("bucket", Options.Bucket);
            // $ must be escaped with backslash.
            policyConditions.AddConditionItem(MatchMode.Exact, PolicyConditions.CondKey, fileKey);
            policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, maxSize); //大小限制

            var postPolicy = Client.GeneratePostPolicy(expiration, policyConditions);
            var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));

            var signature = ComputeSignature(Options.AccessKey, encPolicy);

            var host = (Options.CdnHost ?? Options.Host).TrimEnd('/');
            return new AliyunOssPostPolicy
            {
                AccessId = Options.AccessId,
                Expire = expiration.GetIntDate(),
                Policy = encPolicy,
                Signature = signature,
                Host = Options.Host,
                FileName = fileKey,
                Dir = Path.GetDirectoryName(fileKey),
                Url = $"{host}/{fileKey}"
            };
        }

        public AliyunOssPostPolicy BuildStartWithPolicy(string fileKeyStartWith, int expireSeconds = 60,
            int maxSize = 8 * 1024 * 1024)
        {
            var expiration = DateTime.Now.AddSeconds(expireSeconds);
            var policyConditions = new PolicyConditions();
            policyConditions.AddConditionItem("bucket", Options.Bucket);
            // $ must be escaped with backslash.
            policyConditions.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, fileKeyStartWith);
            policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, maxSize); //大小限制

            var postPolicy = Client.GeneratePostPolicy(expiration, policyConditions);
            var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));

            var signature = ComputeSignature(Options.AccessKey, encPolicy);

            var host = (Options.CdnHost ?? Options.Host).TrimEnd('/');
            return new AliyunOssPostPolicy
            {
                AccessId = Options.AccessId,
                Expire = expiration.GetIntDate(),
                Policy = encPolicy,
                Signature = signature,
                Host = Options.Host,
                Dir = fileKeyStartWith
            };
        }

        private static string ComputeSignature(string key, string data)
        {
            using var algorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant());
            algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
            return Convert.ToBase64String(
                algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
        }

        public string Upload(string objectName, string localFullFileName)
        {
            Client.PutObject(Options.Bucket, objectName, localFullFileName);

            var host = (Options.CdnHost ?? Options.Host).TrimEnd('/');

            return $"{host}/{objectName}";
        }

        public string Upload(string objectName, byte[] fileContent)
        {
            using var requestContent = new MemoryStream(fileContent);
            // 上传文件。
            Client.PutObject(Options.Bucket, objectName, requestContent);
            var host = (Options.CdnHost ?? Options.Host).TrimEnd('/');
            return $"{host}/{objectName}";
        }

        public string SignatureUrl(string objectName, int expires = 3600)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(objectName));

            var request = new GeneratePresignedUriRequest(BucketName, objectName, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddSeconds(expires)
            };
            request.ResponseHeaders.ContentDisposition = $"inline;filename={objectName}";
            return Client.GeneratePresignedUri(request).ToString();
        }

        public Stream Download(string objectName)
        {
            var ossObject = Client.GetObject(BucketName, objectName);
            return ossObject.Content;
        }

        public bool Exists(string objectName)
        {
            return Client.DoesObjectExist(BucketName, objectName);
        }
    }
}